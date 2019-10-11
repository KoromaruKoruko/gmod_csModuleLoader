using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.InteropServices;

namespace GMLoaded.Native
{
    /// <summary>
    /// <para>A handle to safely temporarily hook a method.</para>
    /// <para>The hook is reset when the handle is disposed.</para>
    /// </summary>
    /// <remarks>AnotherForm of Function Injection, Saifer then Direct Injection, but can cause other problems</remarks>
    public class HookHandle : IDisposable
    {
        private static readonly HashSet<HookHandle> HookHandles = new HashSet<HookHandle>();

        private Byte[] _HookedIntro;
        private Byte[] _OriginalIntro;
        private Boolean Disposed;

        /// <summary>
        /// Creates a new <see cref="HookHandle"/> instance with the given parameters.
        /// </summary>
        /// <param name="hookedMethod">The hooked method.</param>
        /// <param name="originalIntro">The original intro of the hooked method.</param>
        /// <param name="hookedIntro">The custom into of the hooked method.</param>
        public HookHandle(IntPtr hookedFunc, Byte[] originalIntro, Byte[] hookedIntro) => this.Init(hookedFunc, originalIntro, hookedIntro);

        public HookHandle(IntPtr hookedFunc, IntPtr newFunc)
        {
            Byte[] NewIntro = new Byte[] { 0xE9, 0x90, 0x90, 0x90, 0x90, 0xC3 };
            Array.Copy(BitConverter.GetBytes(newFunc.ToInt64() - hookedFunc.ToInt64() - 5), 0, NewIntro, 1, 4);
            Byte[] OrigIntro;

            using (Natives.GenericMemory.Protect(hookedFunc, NewIntro.Length, Natives.GenericMemory.GMemProtectionFlags.ALL))
            {
                OrigIntro = new Byte[NewIntro.Length];
                Marshal.Copy(hookedFunc, OrigIntro, 0, OrigIntro.Length);
            }
            this.Init(hookedFunc, OrigIntro, NewIntro);
        }

        public Boolean Hooked
        {
            get;
            private set;
        }

        /// <summary>
        /// The hooked method.
        /// </summary>
        public IntPtr HookedFunc
        {
            get;
            private set;
        }

        private void Init(IntPtr hookedFunc, Byte[] originalIntro, Byte[] hookedIntro)
        {
            this.HookedFunc = hookedFunc;
            this._OriginalIntro = originalIntro;
            this._HookedIntro = hookedIntro;
            HookHandles.Add(this);
            //Hook();
        }

        private void WriteIntro(Byte[] Intro)
        {
            using (Natives.GenericMemory.Protect(this.HookedFunc, Intro.Length, Natives.GenericMemory.GMemProtectionFlags.ALL))
                Marshal.Copy(Intro, 0, this.HookedFunc, Intro.Length);
        }

        public static HookHandle CreateHook(IntPtr Old, IntPtr New) => new HookHandle(Old, New);

        public static HookHandle CreateHook(MethodInfo Old, IntPtr New) => CreateHook(Old.MethodHandle.GetFunctionPointer(), New);

        public static HookHandle CreateHook<THooked>(THooked Old, IntPtr New) => CreateHook((Old as Delegate).Method, New);

        public static HookHandle CreateHook(Expression<Action> OldExpr, IntPtr New)
        {
            if ((OldExpr.Body is MethodCallExpression) == false)
                throw new ArgumentException("Expression body isn't a method!", "OldExpr");
            return CreateHook(((MethodCallExpression)OldExpr.Body).Method, New);
        }

        public static HookHandle CreateHook(MethodInfo Old, MethodInfo New) => CreateHook(Old.MethodHandle.GetFunctionPointer(), New.MethodHandle.GetFunctionPointer());

        public static HookHandle CreateHook(Expression<Action> OldExpr, MethodInfo New)
        {
            if ((OldExpr.Body is MethodCallExpression) == false)
                throw new ArgumentException("Expression body isn't a method!", "OldExpr");
            return CreateHook(((MethodCallExpression)OldExpr.Body).Method, New);
        }

        public static HookHandle CreateHook(Expression<Action> OldExpr, Expression<Action> NewExpr)
        {
            if ((NewExpr.Body is MethodCallExpression) == false)
                throw new ArgumentException("Expression body isn't a method!", "NewExpr");
            return CreateHook(OldExpr, ((MethodCallExpression)NewExpr.Body).Method);
        }

        public static HookHandle CreateHook<T>(IntPtr Old, T New)
        {
            if (typeof(Delegate).IsAssignableFrom(typeof(T)) == false)
                throw new InvalidOperationException("T must be a Delegate type.");
            IntPtr NewPtr = ((Delegate)(Object)New).Method.MethodHandle.GetFunctionPointer();
            return CreateHook(Old, NewPtr);
        }

        public static HookHandle CreateHook(String Module, String Func, IntPtr New)
        {
            IntPtr ModuleHandle = Natives.LoadDL(Module);
            if (ModuleHandle == IntPtr.Zero)
                throw new Exception("Could not get module " + Module);
            IntPtr ProcAddr = Natives.GetDLSymbol(ModuleHandle, Func);
            if (ProcAddr == IntPtr.Zero)
                throw new Exception("Could not get function " + Func);
            return CreateHook(ProcAddr, New);
        }

        /// <summary>
        /// Modifies the underlying method of <paramref name="old"/> to immediately jump to <paramref name="new"/>.
        /// </summary>
        /// <typeparam name="T">A delegate type matching the methods to hook together.</typeparam>
        /// <param name="old">An instance of <typeparamref name="T"/> pointing to the method to hook.</param>
        /// <param name="new">An instance of <typeparamref name="T"/> pointing to the hook target.</param>
        public static HookHandle CreateHook<T>(T old, T @new)
        {
            if (typeof(Delegate).IsAssignableFrom(typeof(T)) == false)
                throw new InvalidOperationException("T must be a Delegate type.");
            MethodInfo oldMethod = ((Delegate)(Object)old).Method;
            MethodInfo newMethod = ((Delegate)(Object)@new).Method;

            if (oldMethod.IsStatic != newMethod.IsStatic)
                throw new ArgumentException("OldFunc and NewFunc must be either both static or both instance methods for this Hook overload.");
            if (oldMethod.IsStatic == false && newMethod.DeclaringType.IsAssignableFrom(oldMethod.DeclaringType) == false)
                throw new ArgumentException("\"This\" parameter type mismatch.");

            return CreateHook(oldMethod, newMethod);
        }

        /// <summary>
        /// Modifies the underlying method of <paramref name="old"/> to immediately jump to <paramref name="new"/>.
        /// </summary>
        /// <typeparam name="THooked">A delegate type matching the method to hook.</typeparam>
        /// <typeparam name="THook">A delegate type matching the method hooked into <paramref name="old"/>.</typeparam>
        /// <param name="old">An instance of <typeparamref name="THooked"/> pointing to the method to hook.</param>
        /// <param name="new">An instance of <typeparamref name="THook"/> pointing to the hook target.</param>
        public static HookHandle CreateHook<THooked, THook>(THooked old, THook @new)
        {
            if (typeof(Delegate).IsAssignableFrom(typeof(THooked)) == false)
                throw new InvalidOperationException("THooked must be a Delegate type.");
            if (typeof(Delegate).IsAssignableFrom(typeof(THook)) == false)
                throw new InvalidOperationException("THook must be a Delegate type.");
            MethodInfo oldMethod = ((Delegate)(Object)old).Method;
            MethodInfo newMethod = ((Delegate)(Object)@new).Method;

            if (oldMethod.ReturnType.IsAssignableFrom(newMethod.ReturnType))
                throw new ArgumentException("Return type mismatch: " + newMethod.ReturnType + " isn't assignable to " + oldMethod.ReturnType + ".");

            // TODO: Check if this works despite the apparent mismatch.
            // I think it's impossible due to unbalancing the stack or something along those lines,
            // but that may be not be the case.
            // For whatever reason typeof(object).IsAssignableFrom(typeof(void)) is true.
            if (oldMethod.ReturnType != typeof(void) && newMethod.ReturnType == typeof(void))
                throw new ArgumentException("Return type mismatch: Can't hook method returning void with one that returns something.");

            List<Type> oldParameters = oldMethod.GetParameters().Select(p => p.ParameterType).ToList();
            if (oldMethod.IsStatic == false)
                oldParameters.Insert(0, oldMethod.DeclaringType);

            List<Type> newParameters = newMethod.GetParameters().Select(p => p.ParameterType).ToList();
            if (newMethod.IsStatic == false)
                newParameters.Insert(0, newMethod.DeclaringType);

            //TODO: Check whether a method with fewer parameters can safely be hooked into one with more.
            if (oldParameters.Count != newParameters.Count)
                throw new ArgumentException("Parameter count (eventually including initial \"this\" parameter) mismatch: Tried to hook method with " +
                    oldParameters.Count + " with one with " + newParameters.Count + " parameters.");
            for (Int32 i = 0; i < oldParameters.Count; i++)
            {
                if (newParameters[i].IsAssignableFrom(oldParameters[i]) == false)
                    throw new ArgumentException("Parameter type mismatch: Can't convert assign " +
                        oldParameters[i] + " to " + newParameters[i] + " at position " + i +
                        " (eventually including initial \"this\" parameter).");
            }

            return CreateHook(oldMethod, newMethod);
        }

        public static void DisposeAllHandles()
        {
            foreach (HookHandle H in HookHandles)
                H.Dispose();
            HookHandles.Clear();
        }

        /// <summary>
        /// Restores <see cref="HookedMethod"/> to its previous state.
        /// </summary>
        public void Dispose()
        {
            this.Disposed = true;
            this.Unhook();
        }

        public HookHandle Hook()
        {
            if (this.Hooked || this.Disposed)
                return this;

            this.Hooked = true;
            this.WriteIntro(this._HookedIntro);

            return this;
        }

        public HookHandle Unhook()
        {
            if (!this.Hooked)
                return this;

            this.Hooked = false;
            this.WriteIntro(this._OriginalIntro);

            return this;
        }
    }
}
