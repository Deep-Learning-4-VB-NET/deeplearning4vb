Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Reflection
Imports System.Reflection.Emit
Imports System.Runtime.CompilerServices
Imports DataType = DeepLearning4VB.org.nd4vb.linalg.api.buffer.DataType
Imports INDArray = DeepLearning4VB.org.nd4vb.linalg.api.ndarray.INDArray
Imports DeepLearning4VB.org.nd4vb.linalg.api.ops

Namespace org.nd4vb.linalg.api.ops

    ''' <summary>
    ''' A fake <see cref="INDArray"/> for the op tests, generated at run time with <c>System.Reflection.Emit</c> so it
    ''' follows the interface as it changes: every member calls <see cref="Dispatch"/>, which runs the behaviour
    ''' registered for the member name or throws <see cref="NotImplementedException"/>. Equality and hash codes are
    ''' <see cref="Object"/>'s (reference identity), as the op classes expect from distinct arrays.
    ''' </summary>
    Public NotInheritable Class FakeArray

        Private Shared ReadOnly Behaviours As New ConditionalWeakTable(Of Object, Dictionary(Of String, Func(Of Object(), Object)))()
        Private Shared ReadOnly FakeType As Type = BuildType()

        Private Sub New()
        End Sub

        ''' <summary>A fake array; <paramref name="behaviours"/> maps member names (property getters are <c>get_Name</c>) to results.</summary>
        Public Shared Function Create(Optional ByVal behaviours As IDictionary(Of String, Func(Of Object(), Object)) = Nothing) As INDArray
            Dim arr As INDArray = DirectCast(Activator.CreateInstance(FakeType), INDArray)
            Dim d As New Dictionary(Of String, Func(Of Object(), Object))(StringComparer.Ordinal)
            If behaviours IsNot Nothing Then
                For Each kv As KeyValuePair(Of String, Func(Of Object(), Object)) In behaviours
                    d(kv.Key) = kv.Value
                Next
            End If
            FakeArray.Behaviours.Add(arr, d)
            Return arr
        End Function

        ''' <summary>A fake array that answers the usual metadata questions: scalar or not, data type, compressed (never).</summary>
        Public Shared Function Simple(Optional ByVal isScalar As Boolean = False,
                                      Optional ByVal dtype As DataType = DataType.FLOAT) As INDArray
            Return Create(New Dictionary(Of String, Func(Of Object(), Object)) From {
                {"get_Scalar", Function(a) isScalar},
                {"get_Compressed", Function(a) False},
                {"dataType", Function(a) dtype},
                {"get_R", Function(a) dtype = DataType.FLOAT OrElse dtype = DataType.DOUBLE OrElse dtype = DataType.HALF OrElse dtype = DataType.BFLOAT16},
                {"get_Z", Function(a) dtype = DataType.INT OrElse dtype = DataType.LONG},
                {"get_B", Function(a) dtype = DataType.BOOL}
            })
        End Function

        ''' <summary>Called by the generated members.</summary>
        Public Shared Function Dispatch(ByVal instance As Object, ByVal name As String, ByVal args As Object()) As Object
            Dim d As Dictionary(Of String, Func(Of Object(), Object)) = Nothing
            Dim f As Func(Of Object(), Object) = Nothing
            If Behaviours.TryGetValue(instance, d) AndAlso d.TryGetValue(name, f) Then
                Return f(args)
            End If
            Throw New NotImplementedException("fake INDArray: " & name)
        End Function

        Private Shared Function BuildType() As Type
            Dim ab As AssemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(New AssemblyName("DeepLearning4VB.Tests.FakeArrays"), AssemblyBuilderAccess.Run)
            Dim mb As ModuleBuilder = ab.DefineDynamicModule("FakeArrays")
            Dim tb As TypeBuilder = mb.DefineType("FakeINDArray", TypeAttributes.Public Or TypeAttributes.Class Or TypeAttributes.Sealed)
            Dim dispatchMethod As MethodInfo = GetType(FakeArray).GetMethod(NameOf(Dispatch))
            Dim ifaces As New List(Of Type) From {GetType(INDArray)}
            ifaces.AddRange(GetType(INDArray).GetInterfaces())
            For Each iface As Type In ifaces
                tb.AddInterfaceImplementation(iface)
                For Each m As MethodInfo In iface.GetMethods()
                    Dim ps As ParameterInfo() = m.GetParameters()
                    Dim ptypes As Type() = ps.Select(Function(p) p.ParameterType).ToArray()
                    Dim meth As MethodBuilder = tb.DefineMethod(iface.FullName & "." & m.Name,
                        MethodAttributes.Private Or MethodAttributes.Virtual Or MethodAttributes.Final Or MethodAttributes.HideBySig Or MethodAttributes.NewSlot,
                        m.ReturnType, ptypes)
                    Dim il As ILGenerator = meth.GetILGenerator()
                    il.Emit(OpCodes.Ldarg_0)
                    il.Emit(OpCodes.Ldstr, m.Name)
                    il.Emit(OpCodes.Ldc_I4, ps.Length)
                    il.Emit(OpCodes.Newarr, GetType(Object))
                    For i As Integer = 0 To ps.Length - 1
                        il.Emit(OpCodes.Dup)
                        il.Emit(OpCodes.Ldc_I4, i)
                        il.Emit(OpCodes.Ldarg, CShort(i + 1))
                        If ptypes(i).IsValueType Then
                            il.Emit(OpCodes.Box, ptypes(i))
                        End If
                        il.Emit(OpCodes.Stelem_Ref)
                    Next
                    il.Emit(OpCodes.Call, dispatchMethod)
                    If m.ReturnType Is GetType(Void) Then
                        il.Emit(OpCodes.Pop)
                    ElseIf m.ReturnType.IsValueType Then
                        il.Emit(OpCodes.Unbox_Any, m.ReturnType)
                    Else
                        il.Emit(OpCodes.Castclass, m.ReturnType)
                    End If
                    il.Emit(OpCodes.Ret)
                    tb.DefineMethodOverride(meth, m)
                Next
            Next
            Return tb.CreateType()
        End Function

    End Class

    ''' <summary>
    ''' Helpers for the wave-2 stand-ins of <c>Nd4vb</c>, <c>Shape</c>, <c>Broadcast</c> and <c>LinAlgExceptions</c>
    ''' (they throw <see cref="NotImplementedException"/> with "stand-in" in the message). With the real classes the
    ''' tolerated calls simply succeed, so the assertions after them hold in both cases.
    ''' </summary>
    Friend Module OpTestSupport

        ''' <summary>True when the exception (or an inner one) comes from a stand-in.</summary>
        Public Function IsStandIn(ByVal ex As Exception) As Boolean
            Dim e As Exception = ex
            Do While e IsNot Nothing
                If TypeOf e Is NotImplementedException AndAlso e.Message.Contains("stand-in") Then
                    Return True
                End If
                e = e.InnerException
            Loop
            Return False
        End Function

        ''' <summary>Runs the action; an exception from a stand-in is swallowed.</summary>
        Public Sub TolerateStandIn(ByVal action As Action)
            Try
                action()
            Catch ex As Exception When IsStandIn(ex)
            End Try
        End Sub

        ''' <summary>
        ''' An instance from the parameterless constructor; when the constructor needs a stand-in (the scalar ops call
        ''' <c>Nd4vb.scalar</c>), an uninitialized instance, which still answers <c>opName</c>, <c>opNum</c> and
        ''' <c>getOpType</c>.
        ''' </summary>
        Public Function CreateOp(ByVal t As Type, ByRef constructed As Boolean) As Object
            Try
                constructed = True
                Return Activator.CreateInstance(t)
            Catch ex As Exception When IsStandIn(ex)
                constructed = False
#If NETFRAMEWORK Then
                Return System.Runtime.Serialization.FormatterServices.GetUninitializedObject(t)
#Else
                Return RuntimeHelpers.GetUninitializedObject(t)
#End If
            End Try
        End Function

        Public Function CreateOp(Of T)() As T
            Dim constructed As Boolean
            Return DirectCast(CreateOp(GetType(T), constructed), T)
        End Function

    End Module

End Namespace
