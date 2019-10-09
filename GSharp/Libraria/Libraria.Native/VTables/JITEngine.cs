﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.InteropServices;
using System.Text;

namespace Libraria.Native {
	[UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
	public delegate IntPtr CreateInterfaceDelegate(string version, IntPtr returnCode);

	public class InterfaceVersions {
		public static string GetInterfaceIdentifier(Type targetClass) {
			foreach (InterfaceVersionAttribute attribute in targetClass.GetCustomAttributes(typeof(InterfaceVersionAttribute), false)) {
				return attribute.Identifier;
			}

			throw new Exception("Version identifier not found for class " + targetClass);
		}
	}

	public class JITEngineException : Exception {
		public JITEngineException(string message) : base(message) { }
	}

	public static class JITEngine {
		private static ModuleBuilder moduleBuilder;
		private static Dictionary<string, object> classCache;

		static JITEngine() {
			classCache = new Dictionary<string, object>();
			AssemblyBuilder assemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(new AssemblyName("JIT"), AssemblyBuilderAccess.RunAndSave);

#if DEBUG
			Type daType = typeof(DebuggableAttribute);
			ConstructorInfo daCtor = daType.GetConstructor(new Type[] { typeof(DebuggableAttribute.DebuggingModes) });
			CustomAttributeBuilder daBuilder = new CustomAttributeBuilder(daCtor, new object[] {
			DebuggableAttribute.DebuggingModes.DisableOptimizations |
			DebuggableAttribute.DebuggingModes.Default });
			assemblyBuilder.SetCustomAttribute(daBuilder);
#endif

			moduleBuilder = assemblyBuilder.DefineDynamicModule("JIT", true);
		}

		private static void EmitPrettyLoad(ILGenerator ilgen, int index) {
			switch (index) {
				case 0: ilgen.Emit(OpCodes.Ldarg_0); break;
				case 1: ilgen.Emit(OpCodes.Ldarg_1); break;
				case 2: ilgen.Emit(OpCodes.Ldarg_2); break;
				case 3: ilgen.Emit(OpCodes.Ldarg_3); break;
				default: ilgen.Emit(OpCodes.Ldarg_S, (byte)index); break;
			}
		}

		private static void EmitPrettyLoadLocal(ILGenerator ilgen, int index) {
			switch (index) {
				case 0: ilgen.Emit(OpCodes.Ldloc_0); break;
				case 1: ilgen.Emit(OpCodes.Ldloc_1); break;
				case 2: ilgen.Emit(OpCodes.Ldloc_2); break;
				case 3: ilgen.Emit(OpCodes.Ldloc_3); break;
				default: ilgen.Emit(OpCodes.Ldloc_S, (byte)index); break;
			}
		}

		private static void EmitPrettyStoreLocal(ILGenerator ilgen, int index) {
			switch (index) {
				case 0: ilgen.Emit(OpCodes.Stloc_0); break;
				case 1: ilgen.Emit(OpCodes.Stloc_1); break;
				case 2: ilgen.Emit(OpCodes.Stloc_2); break;
				case 3: ilgen.Emit(OpCodes.Stloc_3); break;
				default: ilgen.Emit(OpCodes.Stloc_S, (byte)index); break;
			}
		}

		private static void EmitPlatformLoad(ILGenerator ilgen, IntPtr pointer) {
			switch (IntPtr.Size) {
				case 4: ilgen.Emit(OpCodes.Ldc_I4, (int)pointer.ToInt32()); break;
				case 8: ilgen.Emit(OpCodes.Ldc_I8, (long)pointer.ToInt64()); break;
				default: throw new JITEngineException("Bad IntPtr size");
			}

			ilgen.Emit(OpCodes.Conv_I);
		}

		public static TClass GetFromFactory<TClass>(CreateInterfaceDelegate factory) where TClass : class {
			if (factory == null) {
				return null;
				//throw new JITEngineException("GetFromFactory called with NULL factory");
			}

			IntPtr classptr = factory(InterfaceVersions.GetInterfaceIdentifier(typeof(TClass)), IntPtr.Zero);
			if (classptr == IntPtr.Zero)
				return null;

			return GenerateClass<TClass>(classptr);
		}

		public static TClass GenerateClass<TClass>(IntPtr ptr) where TClass : class {
			if (ptr == IntPtr.Zero) {
				return null;
				//throw new JITEngineException("GenerateClass called with NULL ptr");
			}

			string uniqueID = ptr.GetHashCode().ToString() + typeof(TClass).GetHashCode().ToString();
			if (classCache.ContainsKey(uniqueID))
				return (TClass)classCache[uniqueID];

			IntPtr vtable_ptr = Marshal.ReadIntPtr(ptr);

			Type targetInterface = typeof(TClass);

			TypeBuilder builder = moduleBuilder.DefineType(targetInterface.Name + "_" + uniqueID,
													TypeAttributes.Class, null, new Type[] { targetInterface });
			builder.AddInterfaceImplementation(targetInterface);

			FieldBuilder fbuilder = builder.DefineField("ObjectAddress", typeof(IntPtr), FieldAttributes.Public);
			builder.DefineField(nameof(VTable), typeof(VTable), FieldAttributes.Public);

			ClassJITInfo classInfo = new ClassJITInfo(targetInterface);

			for (int i = 0; i < classInfo.Methods.Count; i++) {
				IntPtr vtableMethod = Marshal.ReadIntPtr(vtable_ptr, IntPtr.Size * classInfo.Methods[i].VTableSlot);
				MethodJITInfo methodInfo = classInfo.Methods[i];

				EmitClassMethod(methodInfo, ptr, vtableMethod, builder, fbuilder);
			}

			Type implClass = builder.CreateType();
			Object instClass = Activator.CreateInstance(implClass);

			FieldInfo addressField = implClass.GetField("ObjectAddress", BindingFlags.Public | BindingFlags.Instance);
			addressField.SetValue(instClass, ptr);

			classCache.Add(uniqueID, instClass);
			return (TClass)instClass;
		}

		private class MethodState {
			public struct RefArgLocal {
				public LocalBuilder builder;
				public int argIndex;
				public Type paramType;
			}

			public MethodState() {
				MethodArgs = new List<Type>();
				NativeArgs = new List<Type>();
				unmanagedMemory = new List<LocalBuilder>();
				refargLocals = new List<RefArgLocal>();
			}

			public List<Type> MethodArgs;
			public Type MethodReturn;

			public List<Type> NativeArgs;
			public Type NativeReturn;

			public bool ReturnTypeByStack;
			public LocalBuilder localReturn;

			public List<LocalBuilder> unmanagedMemory;
			public List<RefArgLocal> refargLocals;
		}

		private static void EmitClassMethod(MethodJITInfo method, IntPtr objectptr, IntPtr methodptr, TypeBuilder builder, FieldBuilder addressAssistant) {
			MethodState state = new MethodState();

			state.NativeArgs.Add(typeof(IntPtr)); // thisptr

			state.ReturnTypeByStack = method.ReturnType.DetermineProps();

			if (state.ReturnTypeByStack) {
				// ref to the native return type
				state.NativeArgs.Add(method.ReturnType.NativeType.MakeByRefType());
				state.NativeReturn = null;
			} else if (method.ReturnType.IsStringClass) {
				// special case for strings, we will marshal it in ourselves
				state.NativeReturn = typeof(IntPtr);
			} else {
				state.NativeReturn = method.ReturnType.NativeType;
			}

			state.MethodReturn = method.ReturnType.Type;

			foreach (TypeJITInfo typeInfo in method.Args) {
				// populate MethodArgs and NativeArgs now
				typeInfo.DetermineProps();

				state.MethodArgs.Add(typeInfo.Type);

				if (typeInfo.IsStringClass) {
					// we need to specially marshal strings
					state.NativeArgs.Add(typeof(IntPtr));
				} else
				if (!typeInfo.IsParams) {
					if (typeInfo.IsByRef && typeInfo.NativeType.IsValueType) {
						state.NativeArgs.Add(typeInfo.NativeType.MakeByRefType());
					} else {
						state.NativeArgs.Add(typeInfo.NativeType);
					}
				}
			}

			// add a native version parameter if the return type is detected as a versioned class
			if (method.ReturnType.IsGeneric || (method.ReturnType.IsCreatableClass && method.ReturnType.IsInterfaceVersioned)) {
				state.NativeArgs.Add(typeof(string));
			}


			MethodBuilder mbuilder = builder.DefineMethod(method.Name, MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.NewSlot | MethodAttributes.Virtual, CallingConventions.HasThis);

			if (method.ReturnType.IsGeneric) {
				// create generic param
				GenericTypeParameterBuilder[] gtypeParameters;
				gtypeParameters = mbuilder.DefineGenericParameters(new string[] { "TClass" });

				state.MethodReturn = gtypeParameters[0];
				gtypeParameters[0].SetGenericParameterAttributes(GenericParameterAttributes.ReferenceTypeConstraint);
			}

			mbuilder.SetReturnType(state.MethodReturn);
			mbuilder.SetParameters(state.MethodArgs.ToArray());

			builder.DefineMethodOverride(mbuilder, method.MethodInfo);

			ILGenerator ilgen = mbuilder.GetILGenerator();

			// load object pointer
			EmitPlatformLoad(ilgen, objectptr);

			if (state.ReturnTypeByStack) {
				// allocate local to hold the return
				state.localReturn = ilgen.DeclareLocal(method.ReturnType.NativeType);
				state.localReturn.SetLocalSymInfo("nativeReturnPlaceholder");

				ilgen.Emit(OpCodes.Ldloca_S, state.localReturn.LocalIndex);
			}

			int argindex = 0;
			foreach (TypeJITInfo typeInfo in method.Args) {
				argindex++;

				// perform any conversions necessary
				if (typeInfo.NativeType != typeInfo.Type && typeInfo.IsByRef) {
					LocalBuilder localArg = ilgen.DeclareLocal(typeInfo.NativeType);
					localArg.SetLocalSymInfo("byrefarg" + argindex);

					var helper = new MethodState.RefArgLocal();
					helper.builder = localArg;
					helper.argIndex = argindex;
					helper.paramType = typeInfo.PierceType;

					state.refargLocals.Add(helper);
					ilgen.Emit(OpCodes.Ldloca_S, localArg);
				} else if (typeInfo.IsDelegate) {
					throw new Exception("CARTMAN FIX");
					//EmitPrettyLoad(ilgen, argindex);

					//var met = typeof(Marshal).GetMethods().Where(m => m.Name == nameof(Marshal.GetFunctionPointerForDelegate) && m.IsGenericMethod).FirstOrDefault();
					//var genMet = met.MakeGenericMethod(typeInfo.Type);

					//ilgen.EmitCall(OpCodes.Call, genMet, null);
				} else if (typeInfo.NativeType != typeInfo.Type && !typeInfo.Type.IsEnum) {
					EmitPrettyLoad(ilgen, argindex);
					ilgen.EmitCall(OpCodes.Call, typeInfo.Type.GetMethod("GetValue"), null);
				} else {
					EmitPrettyLoad(ilgen, argindex);
				}

				if ((typeInfo.IsStringClass || typeInfo.IsParams) && method.HasParams) {
					if (!typeInfo.IsParams)
						continue;

					ilgen.EmitCall(OpCodes.Call, typeof(String).GetMethod("Format", BindingFlags.Public | BindingFlags.Static, null, new Type[] { typeof(string), typeof(object[]) }, null), null);
				}

				if (typeInfo.IsStringClass || typeInfo.IsParams) {
					LocalBuilder localString = ilgen.DeclareLocal(typeof(GCHandle));
					localString.SetLocalSymInfo("nativeString" + argindex);

					state.unmanagedMemory.Add(localString);

					// we need to specially marshal strings
					ilgen.Emit(OpCodes.Ldloca, localString.LocalIndex);
					ilgen.EmitCall(OpCodes.Call, typeof(InteropHelp).GetMethod("EncodeUTF8String"), null);
				} else if (typeInfo.IsCreatableClass) {
					// if this argument is a class we understand: get the object pointer
					ilgen.Emit(OpCodes.Ldfld, addressAssistant);
				}
			}

			if (method.ReturnType.IsGeneric || (method.ReturnType.IsCreatableClass && method.ReturnType.IsInterfaceVersioned)) {
				ilgen.Emit(OpCodes.Ldtoken, method.ReturnType.Type);
				ilgen.EmitCall(OpCodes.Call, typeof(Type).GetMethod("GetTypeFromHandle", BindingFlags.Static | BindingFlags.Public), null);
				ilgen.EmitCall(OpCodes.Call, typeof(InterfaceVersions).GetMethod("GetInterfaceIdentifier", BindingFlags.Static | BindingFlags.Public), null);
			}

			// load vtable method pointer
			EmitPlatformLoad(ilgen, methodptr);

			CallingConvention ccv = CallingConvention.ThisCall;

			if (method.HasParams)
				ccv = CallingConvention.Cdecl;

			if (state.NativeReturn == typeof(bool))
				state.NativeReturn = typeof(byte);

			ilgen.EmitCalli(OpCodes.Calli, ccv, state.NativeReturn, state.NativeArgs.ToArray());

			// populate byref args
			foreach (var local in state.refargLocals) {
				EmitPrettyLoad(ilgen, local.argIndex);
				EmitPrettyLoadLocal(ilgen, local.builder.LocalIndex);
				ilgen.Emit(OpCodes.Newobj, local.paramType.GetConstructor(new Type[] { local.builder.LocalType }));
				ilgen.Emit(OpCodes.Stind_Ref);
			}

			// clean up unmanaged memory
			foreach (LocalBuilder localbuilder in state.unmanagedMemory) {
				ilgen.Emit(OpCodes.Ldloca, localbuilder.LocalIndex);
				ilgen.EmitCall(OpCodes.Call, typeof(InteropHelp).GetMethod("FreeString"), null);
			}

			if (state.ReturnTypeByStack) {
				EmitPrettyLoadLocal(ilgen, state.localReturn.LocalIndex);

				// reconstruct return type
				if (state.localReturn.LocalType != state.MethodReturn) {
					ilgen.Emit(OpCodes.Newobj, state.MethodReturn.GetConstructor(new Type[] { state.localReturn.LocalType }));
				}
			} else if (method.ReturnType.IsCreatableClass) {
				if (method.ReturnType.IsGeneric) {
					ilgen.EmitCall(OpCodes.Call, typeof(JITEngine).GetMethod("GenerateClass", BindingFlags.Static | BindingFlags.Public), null);
				} else if (method.ReturnType.IsDelegate) {
					ilgen.Emit(OpCodes.Ldtoken, method.ReturnType.Type);
					ilgen.EmitCall(OpCodes.Call, typeof(Type).GetMethod("GetTypeFromHandle"), null);
					ilgen.EmitCall(OpCodes.Call, typeof(Marshal).GetMethod("GetDelegateForFunctionPointer", new Type[] { typeof(IntPtr), typeof(Type) }), null);
					ilgen.Emit(OpCodes.Castclass, method.ReturnType.Type);
				} else {
					ilgen.EmitCall(OpCodes.Call, typeof(JITEngine).GetMethod("GenerateClass", BindingFlags.Static | BindingFlags.Public).MakeGenericMethod(method.ReturnType.Type), null);
				}
			} else if (method.ReturnType.IsStringClass) {
				// marshal string return
				ilgen.EmitCall(OpCodes.Call, typeof(InteropHelp).GetMethod("DecodeUTF8String"), null);
			}

			ilgen.Emit(OpCodes.Ret);
		}
	}
}
