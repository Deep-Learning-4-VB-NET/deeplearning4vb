' DeepLearning4VB: helpers shared by the managed BLAS and LAPACK classes (phase 2, see docs/PHASE2_DESIGN.md).

Imports System
Imports DataBuffer = DeepLearning4VB.org.nd4vb.linalg.api.buffer.DataBuffer
Imports INDArray = DeepLearning4VB.org.nd4vb.linalg.api.ndarray.INDArray
Imports ND4VBArraySizeException = DeepLearning4VB.org.nd4vb.linalg.exception.ND4VBArraySizeException
Imports ND4VBIllegalStateException = DeepLearning4VB.org.nd4vb.linalg.exception.ND4VBIllegalStateException
Imports ManagedDataBuffer = DeepLearning4VB.org.nd4vb.linalg.managed.buffer.ManagedDataBuffer
Imports ManagedStorage = DeepLearning4VB.org.nd4vb.linalg.managed.storage.ManagedStorage
Imports StorageType = DeepLearning4VB.org.nd4vb.linalg.managed.storage.StorageType

Namespace org.nd4vb.linalg.managed.blas

    ''' <summary>
    ''' The typed .NET arrays and element offsets the managed BLAS kernels (<c>ManagedBlas</c>) take, read from an array's
    ''' or a buffer's <see cref="ManagedDataBuffer"/>: the storage and the <see cref="ManagedDataBuffer.StorageOffset"/> of
    ''' element 0 (for an array, its first element: ND4J 1.0.0-M2 views have view buffers, as the native BLAS calls
    ''' assumed with <c>data().addressPointer()</c>).
    ''' </summary>
    Friend NotInheritable Class BlasOperands

        Private Sub New()
        End Sub

        ''' <summary>The managed buffer behind the array or buffer.</summary>
        ''' <exception cref="ND4VBIllegalStateException">It is not a managed buffer.</exception>
        Friend Shared Function managedBuffer(ByVal buffer As DataBuffer, ByVal name As String) As ManagedDataBuffer
            If buffer Is Nothing Then
                Throw New ND4VBIllegalStateException(name & " has no data buffer")
            End If
            Dim managed As ManagedDataBuffer = TryCast(buffer, ManagedDataBuffer)
            If managed Is Nothing Then
                Throw New ND4VBIllegalStateException(name & " is backed by a " & CObj(buffer).GetType().Name & "; the managed BLAS runs only on ManagedDataBuffer data")
            End If
            Return managed
        End Function

        Private Shared Function typedStorage(ByVal buffer As ManagedDataBuffer, ByVal expected As StorageType, ByVal name As String) As ManagedStorage
            Dim s As ManagedStorage = buffer.Storage
            If s.StorageType <> expected Then
                Throw New ND4VBIllegalStateException(name & " must be " & expected.ToString() & " data, got " & s.StorageType.ToString())
            End If
            Return s
        End Function

        ''' <summary>The FLOAT elements behind a buffer and the index of element <paramref name="offset"/> of the buffer.</summary>
        Friend Shared Function floats(ByVal buffer As DataBuffer, ByVal offset As Long, ByVal name As String, ByRef index As Long) As Single()
            Dim managed As ManagedDataBuffer = managedBuffer(buffer, name)
            index = managed.StorageOffset + offset
            Return typedStorage(managed, StorageType.FLOAT, name).Floats
        End Function

        ''' <summary>The DOUBLE elements behind a buffer and the index of element <paramref name="offset"/> of the buffer.</summary>
        Friend Shared Function doubles(ByVal buffer As DataBuffer, ByVal offset As Long, ByVal name As String, ByRef index As Long) As Double()
            Dim managed As ManagedDataBuffer = managedBuffer(buffer, name)
            index = managed.StorageOffset + offset
            Return typedStorage(managed, StorageType.DOUBLE, name).Doubles
        End Function

        ''' <summary>The FLOAT elements behind an array and the index of its first element.</summary>
        Friend Shared Function floats(ByVal arr As INDArray, ByVal name As String, ByRef index As Long) As Single()
            Return floats(arr.data(), 0, name, index)
        End Function

        ''' <summary>The DOUBLE elements behind an array and the index of its first element.</summary>
        Friend Shared Function doubles(ByVal arr As INDArray, ByVal name As String, ByRef index As Long) As Double()
            Return doubles(arr.data(), 0, name, index)
        End Function

        ''' <summary>An element count as the kernels take it.</summary>
        ''' <exception cref="ND4VBArraySizeException">It does not fit an Integer.</exception>
        Friend Shared Function count(ByVal n As Long) As Integer
            If n > Integer.MaxValue Then
                Throw New ND4VBArraySizeException()
            End If
            Return CInt(n)
        End Function

        ''' <summary>The exception for a routine without a managed implementation yet (milestone M2).</summary>
        Friend Shared Function notImplemented(ByVal routine As String) As NotSupportedException
            Return New NotSupportedException(routine & " is not implemented in the managed backend yet (phase 2 milestone M2)")
        End Function

        ''' <summary>The exception for a routine Java's CPU backend did not support either.</summary>
        Friend Shared Function notSupported(ByVal routine As String) As NotSupportedException
            Return New NotSupportedException(routine & " is not supported (the native CPU backend did not support it either)")
        End Function

    End Class

End Namespace
