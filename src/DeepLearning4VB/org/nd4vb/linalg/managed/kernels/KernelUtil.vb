' DeepLearning4VB: argument checks and small helpers shared by the managed kernels.

Imports System
Imports DeepLearning4VB.org.nd4vb.linalg.managed.storage
Imports SysMath = System.Math

Namespace org.nd4vb.linalg.managed.kernels

    ''' <summary>Validation and helpers shared by the kernel classes.</summary>
    Friend NotInheritable Class KernelUtil

        Private Sub New()
        End Sub

        ''' <summary>The extra arguments, or an empty array for Nothing.</summary>
        Friend Shared Function Args(ByVal extraArgs As Double()) As Double()
            If extraArgs Is Nothing Then
                Return Array.Empty(Of Double)()
            End If
            Return extraArgs
        End Function

        ''' <summary>Checks an operand: storage present, well-formed shape info, and every element inside the storage.</summary>
        Friend Shared Sub CheckOperand(ByVal s As ManagedStorage, ByVal offset As Long, ByVal info As Long(), ByVal name As String)
            If s Is Nothing Then
                Throw New ArgumentNullException(name)
            End If
            ShapeInfoHelper.Validate(info, name & "ShapeInfo")
            If offset < 0 Then
                Throw New ArgumentOutOfRangeException(name & "Offset", "Offset must not be negative")
            End If
            Dim r As Integer = CInt(info(0))
            Dim lo As Long = offset
            Dim hi As Long = offset
            For d As Integer = 0 To r - 1
                Dim size As Long = info(1 + d)
                If size = 0 Then
                    Return
                End If
                If size < 0 Then
                    Throw New ArgumentException("Negative dimension in " & name & " shape", name & "ShapeInfo")
                End If
                Dim extent As Long = (size - 1) * info(1 + r + d)
                If extent > 0 Then
                    hi += extent
                Else
                    lo += extent
                End If
            Next
            If lo < 0 OrElse hi >= s.Length Then
                Throw New ArgumentException("The view of " & name & " (offset " & offset & ", elements " & lo & " to " & hi & ") lies outside its storage of length " & s.Length, name)
            End If
        End Sub

        ''' <summary>Checks an operand that numeric kernels read or write (not UTF8).</summary>
        Friend Shared Sub CheckNumericOperand(ByVal s As ManagedStorage, ByVal offset As Long, ByVal info As Long(), ByVal name As String)
            CheckOperand(s, offset, info, name)
            If s.StorageType = StorageType.UTF8 Then
                Throw New NotSupportedException("UTF8 storage (" & name & ") cannot be used by numeric kernels")
            End If
        End Sub

        ''' <summary>Throws unless the two operands have the same number of elements.</summary>
        Friend Shared Sub CheckSameLength(ByVal aInfo As Long(), ByVal aName As String, ByVal bInfo As Long(), ByVal bName As String)
            Dim a As Long = ShapeInfoHelper.Length(aInfo)
            Dim b As Long = ShapeInfoHelper.Length(bInfo)
            If a <> b Then
                Throw New ArgumentException(aName & " has " & a & " elements but " & bName & " has " & b)
            End If
        End Sub

        ''' <summary>
        ''' True when the op is integer arithmetic in libnd4j's sense: z and every input are integer
        ''' storages (signed or unsigned, not BOOL). Such results wrap around like Java's integer
        ''' arithmetic (see <see cref="StoreResult"/>); with any floating-point or BOOL operand the result
        ''' is a cast and saturates.
        ''' </summary>
        Friend Shared Function IsIntegerOp(ByVal z As ManagedStorage, ByVal ParamArray inputs As ManagedStorage()) As Boolean
            If Not StorageTypes.IsInteger(z.StorageType) Then
                Return False
            End If
            For Each s As ManagedStorage In inputs
                If s IsNot Nothing AndAlso Not StorageTypes.IsInteger(s.StorageType) Then
                    Return False
                End If
            Next
            Return True
        End Function

        ''' <summary>
        ''' Stores a result computed in double. For an integer op (<paramref name="wrap"/>) a finite value
        ''' inside the signed 64-bit range is truncated to a long and narrowed with wrap-around (Java's
        ''' <c>(int) (long) v</c>, so 127 + 1 stored in a BYTE gives -128); anything else, and every
        ''' non-integer op, goes through <see cref="ManagedStorage.SetDouble"/> (Java's saturating cast).
        ''' </summary>
        Friend Shared Sub StoreResult(ByVal z As ManagedStorage, ByVal index As Long, ByVal value As Double, ByVal wrap As Boolean)
            If wrap AndAlso value > -TwoPow63 AndAlso value < TwoPow63 Then
                z.SetLong(index, CLng(SysMath.Truncate(value)))
            Else
                z.SetDouble(index, value)
            End If
        End Sub

        Private Const TwoPow63 As Double = 9.2233720368547758E+18

        ''' <summary>A dense row-major rank-1 shape info of length <paramref name="n"/>.</summary>
        Friend Shared Function VectorInfo(ByVal n As Long) As Long()
            Return ShapeInfoHelper.Create(New Long() {n}, "c"c)
        End Function

        ''' <summary>
        ''' When the operand is not linearly strided, copies it into a dense rank-1 storage of the same type
        ''' (row-major logical order) and returns that; otherwise returns the operand unchanged.
        ''' </summary>
        Friend Shared Sub Densify(ByVal s As ManagedStorage, ByVal offset As Long, ByVal info As Long(), ByRef ds As ManagedStorage, ByRef dOffset As Long, ByRef dInfo As Long())
            If ShapeInfoHelper.LinearStride(info) <> 0 Then
                ds = s
                dOffset = offset
                dInfo = info
                Return
            End If
            Dim n As Long = ShapeInfoHelper.Length(info)
            ds = ManagedStorage.Create(s.StorageType, n)
            dOffset = 0
            dInfo = VectorInfo(n)
            ElementWiseKernels.Copy(s, offset, info, ds, 0, dInfo)
        End Sub

    End Class

End Namespace
