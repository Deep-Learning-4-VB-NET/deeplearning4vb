Imports System
Imports ND4VBIllegalStateException = DeepLearning4VB.org.nd4vb.linalg.exception.ND4VBIllegalStateException

Namespace org.nd4vb.linalg.api.ops.executioner

    ''' <summary>
    ''' The status codes of op execution (Java <c>OpStatus</c>; the constants are renamed <c>ND4VB_STATUS_*</c> like
    ''' the other components). The values are the Java ordinals; the libnd4j status numbers are mapped by
    ''' <see cref="OpStatusExtensions.OpStatusByNumber"/>.
    ''' </summary>
    Public Enum OpStatus
        ND4VB_STATUS_OK
        ND4VB_STATUS_BAD_INPUT
        ND4VB_STATUS_BAD_SHAPE
        ND4VB_STATUS_BAD_RANK
        ND4VB_STATUS_BAD_PARAMS
        ND4VB_STATUS_BAD_OUTPUT
        ND4VB_STATUS_BAD_RNG
        ND4VB_STATUS_BAD_EPSILON
        ND4VB_STATUS_BAD_GRADIENTS
        ND4VB_STATUS_BAD_BIAS
        ND4VB_STATUS_BAD_GRAPH
        ND4VB_STATUS_BAD_LENGTH
        ND4VB_STATUS_BAD_DIMENSIONS
        ND4VB_STATUS_BAD_ORDER
        ND4VB_STATUS_BAD_ARGUMENTS
        ND4VB_STATUS_VALIDATION
    End Enum

    ''' <summary>The static methods of the Java <c>OpStatus</c> enum.</summary>
    Public Module OpStatusExtensions

        ''' <summary>The status for a libnd4j status number (Java <c>OpStatus.byNumber</c>).</summary>
        Public Function OpStatusByNumber(ByVal val As Integer) As OpStatus
            Select Case val
                Case 0
                    Return OpStatus.ND4VB_STATUS_OK
                Case 1
                    Return OpStatus.ND4VB_STATUS_BAD_INPUT
                Case 2
                    Return OpStatus.ND4VB_STATUS_BAD_SHAPE
                Case 3
                    Return OpStatus.ND4VB_STATUS_BAD_RANK
                Case 4
                    Return OpStatus.ND4VB_STATUS_BAD_PARAMS
                Case 5
                    Return OpStatus.ND4VB_STATUS_BAD_OUTPUT
                Case 6
                    Return OpStatus.ND4VB_STATUS_BAD_RNG
                Case 7
                    Return OpStatus.ND4VB_STATUS_BAD_EPSILON
                Case 8
                    Return OpStatus.ND4VB_STATUS_BAD_GRADIENTS
                Case 9
                    Return OpStatus.ND4VB_STATUS_BAD_BIAS
                Case 20
                    Return OpStatus.ND4VB_STATUS_VALIDATION
                Case 30
                    Return OpStatus.ND4VB_STATUS_BAD_GRAPH
                Case 31
                    Return OpStatus.ND4VB_STATUS_BAD_LENGTH
                Case 32
                    Return OpStatus.ND4VB_STATUS_BAD_DIMENSIONS
                Case 33
                    Return OpStatus.ND4VB_STATUS_BAD_ORDER
                Case 34
                    Return OpStatus.ND4VB_STATUS_BAD_ARGUMENTS
                Case Else
                    Throw New ND4VBIllegalStateException("Unknown status given: " & val)
            End Select
        End Function

        ''' <summary>Java <c>OpStatus.values()</c>.</summary>
        Public Function OpStatusValues() As OpStatus()
            Return DirectCast([Enum].GetValues(GetType(OpStatus)), OpStatus())
        End Function

        ''' <summary>Parses the constant name (Java <c>OpStatus.valueOf</c>); numeric strings are rejected.</summary>
        Public Function OpStatusValueOf(ByVal name As String) As OpStatus
            Dim result As OpStatus
            If name IsNot Nothing AndAlso name.Length > 0 AndAlso Char.IsLetter(name(0)) AndAlso [Enum].TryParse(name, False, result) AndAlso [Enum].IsDefined(GetType(OpStatus), result) Then
                Return result
            End If
            Throw New ArgumentException("No enum constant OpStatus." & name)
        End Function

    End Module

End Namespace
