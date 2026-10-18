Imports System
Imports DeepLearning4VB.org.nd4vb.common.function

Namespace org.nd4vb.linalg.indexing.conditions

    ''' <summary>
    ''' Sets a condition in correspondence with the MatchConditionalBool op
    ''' (op num 5 in the legacy operations)
    '''
    ''' Condition number is affected by the ops internals, see here for the comprehensive overview:
    ''' https://github.com/eclipse/deeplearning4j/blob/master/libnd4j/include/ops/ops.h#L2253
    '''
    ''' As of this writing (July 27,2021), the following operations are relevant:
    ''' 0: equals
    ''' 1: not equals
    ''' 2: less than
    ''' 3: greater than
    ''' 4: less than or equal
    ''' 5: greater than or equal
    ''' 6: absolute difference less than
    ''' 7: absolute difference greater than
    ''' 8: is infinite
    ''' 9: is nan
    ''' 10: absolute equals
    ''' 11: not equals
    ''' 12: absolute difference greater or equal to
    ''' 13: absolute difference less than or equal to
    ''' 14: is finite
    ''' 15: is infinite
    ''' <para>
    ''' The inherited <c>apply(Object)</c> evaluates the condition for one value (Java <c>apply(Number)</c>; Java
    ''' <c>Number</c> is <see cref="Object"/> holding a boxed .NET number).
    ''' </para>
    ''' </summary>
    Public Interface Condition
        Inherits [Function](Of Object, Boolean)

        ''' <summary>
        ''' Allows overriding of the value. Java declares this as a default (no-op) method for aggregate conditions,
        ''' mainly to support calls such as <c>INDArray.match(input, Conditions.equals())</c>
        ''' (https://github.com/eclipse/deeplearning4j/issues/9393). VB interfaces have no default methods, so every
        ''' implementation provides it (a no-op where the condition has no value).
        ''' </summary>
        ''' <param name="value"> the new value (a boxed .NET number) </param>
        Sub setValue(ByVal value As Object)

        ''' <summary>
        ''' Returns condition ID for native side (Java spells it <c>condtionNum</c>).
        ''' </summary>
        Function condtionNum() As Integer

        ''' <summary>The value the condition compares with.</summary>
        ReadOnly Property Value As Double

        ''' <summary>The epsilon used by the equality conditions.</summary>
        Function epsThreshold() As Double

    End Interface

End Namespace
