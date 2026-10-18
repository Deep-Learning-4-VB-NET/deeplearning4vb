' DeepLearning4VB: the condition modes of ND4J's Conditions, as evaluated by libnd4j (phase 2, see docs/PHASE2_DESIGN.md).
' New code, no Java original. The mode numbers are those of org.nd4j.linalg.indexing.conditions.*.condtionNum().

Imports System
Imports SysMath = System.Math

Namespace org.nd4vb.linalg.managed.ops

    ''' <summary>
    ''' Evaluates an ND4J condition the way libnd4j's <c>MatchCondition</c> op does. The ops that take a condition
    ''' (<c>match_condition</c>, <c>match_condition_transform</c>, <c>first_index</c>, <c>last_index</c>, <c>cas</c>, <c>car</c>)
    ''' carry it in their extra arguments as a compare value, an epsilon and the mode number of the Java
    ''' <c>Condition.condtionNum()</c>.
    ''' </summary>
    Public NotInheritable Class ConditionMath

        Private Sub New()
        End Sub

        ''' <summary><c>|x - compare| &lt;= eps</c> (EpsilonEquals, and EqualsCondition, which also reports mode 0).</summary>
        Public Const ModeEpsEquals As Integer = 0
        ''' <summary><c>|x - compare| &gt; eps</c> (EpsilonNotEquals).</summary>
        Public Const ModeEpsNotEquals As Integer = 1
        ''' <summary><c>x &lt; compare</c>.</summary>
        Public Const ModeLessThan As Integer = 2
        ''' <summary><c>x &gt; compare</c>.</summary>
        Public Const ModeGreaterThan As Integer = 3
        ''' <summary><c>x &lt;= compare</c>.</summary>
        Public Const ModeLessThanOrEqual As Integer = 4
        ''' <summary><c>x &gt;= compare</c>.</summary>
        Public Const ModeGreaterThanOrEqual As Integer = 5
        ''' <summary><c>|x| &lt; compare</c>.</summary>
        Public Const ModeAbsLessThan As Integer = 6
        ''' <summary><c>|x| &gt; compare</c>.</summary>
        Public Const ModeAbsGreaterThan As Integer = 7
        ''' <summary>x is ±Infinity.</summary>
        Public Const ModeIsInfinite As Integer = 8
        ''' <summary>x is NaN.</summary>
        Public Const ModeIsNaN As Integer = 9
        ''' <summary><c>x == compare</c> (exact).</summary>
        Public Const ModeEquals As Integer = 10
        ''' <summary><c>x != compare</c> (exact; NotEqualsCondition).</summary>
        Public Const ModeNotEquals As Integer = 11
        ''' <summary><c>|x| &gt;= compare</c>.</summary>
        Public Const ModeAbsGreaterThanOrEqual As Integer = 12
        ''' <summary><c>|x| &lt;= compare</c>.</summary>
        Public Const ModeAbsLessThanOrEqual As Integer = 13
        ''' <summary>x is neither infinite nor NaN.</summary>
        Public Const ModeIsFinite As Integer = 14
        ''' <summary>x is infinite or NaN.</summary>
        Public Const ModeNotFinite As Integer = 15

        ''' <summary>
        ''' True when <paramref name="x"/> satisfies the condition. Comparisons with NaN are false, as in C, so NaN
        ''' only matches modes 9 (is NaN), 11 (not equals) and 15 (not finite).
        ''' libnd4j prints a message and returns the input for an unknown mode; this throws instead.
        ''' </summary>
        ''' <exception cref="ArgumentOutOfRangeException">The mode is not 0 to 15.</exception>
        Public Shared Function Matches(ByVal mode As Integer, ByVal x As Double, ByVal compare As Double, ByVal eps As Double) As Boolean
            Select Case mode
                Case ModeEpsEquals
                    Return SysMath.Abs(x - compare) <= eps
                Case ModeEpsNotEquals
                    Return SysMath.Abs(x - compare) > eps
                Case ModeLessThan
                    Return x < compare
                Case ModeGreaterThan
                    Return x > compare
                Case ModeLessThanOrEqual
                    Return x <= compare
                Case ModeGreaterThanOrEqual
                    Return x >= compare
                Case ModeAbsLessThan
                    Return SysMath.Abs(x) < compare
                Case ModeAbsGreaterThan
                    Return SysMath.Abs(x) > compare
                Case ModeIsInfinite
                    Return Double.IsInfinity(x)
                Case ModeIsNaN
                    Return Double.IsNaN(x)
                Case ModeEquals
                    Return x = compare
                Case ModeNotEquals
                    Return x <> compare
                Case ModeAbsGreaterThanOrEqual
                    Return SysMath.Abs(x) >= compare
                Case ModeAbsLessThanOrEqual
                    Return SysMath.Abs(x) <= compare
                Case ModeIsFinite
                    Return Not (Double.IsInfinity(x) OrElse Double.IsNaN(x))
                Case ModeNotFinite
                    Return Double.IsInfinity(x) OrElse Double.IsNaN(x)
                Case Else
                    Throw New ArgumentOutOfRangeException(NameOf(mode), mode, "Unknown condition mode " & mode & " (ND4J condition modes are 0 to 15)")
            End Select
        End Function

        ''' <summary>The mode number stored as a double in an op's extra arguments (truncated, like C's cast to int).</summary>
        Public Shared Function ModeOf(ByVal value As Double) As Integer
            If Double.IsNaN(value) OrElse value >= 2147483648.0 OrElse value <= -2147483649.0 Then Return -1
            Return CInt(SysMath.Truncate(value))
        End Function

        ''' <summary>
        ''' Evaluates the condition stored in <paramref name="extraArgs"/> as <c>{compare, eps, mode}</c> (the layout of
        ''' <c>match_condition</c>, <c>match_condition_transform</c>, <c>first_index</c> and <c>last_index</c>).
        ''' </summary>
        Public Shared Function MatchesArgs(ByVal x As Double, ByVal extraArgs As Double(), ByVal opName As String) As Boolean
            If extraArgs Is Nothing OrElse extraArgs.Length < 3 Then
                Throw New ArgumentException("Op '" & opName & "' needs the extra arguments {compare, eps, mode}", NameOf(extraArgs))
            End If
            Return Matches(ModeOf(extraArgs(2)), x, extraArgs(0), extraArgs(1))
        End Function

        ''' <summary>
        ''' Evaluates the condition of <c>cas</c>/<c>car</c>, whose extra arguments are <c>{compare, set, eps, mode}</c>.
        ''' </summary>
        Public Shared Function MatchesCompareAndSetArgs(ByVal x As Double, ByVal extraArgs As Double(), ByVal opName As String) As Boolean
            If extraArgs Is Nothing OrElse extraArgs.Length < 4 Then
                Throw New ArgumentException("Op '" & opName & "' needs the extra arguments {compare, set, eps, mode}", NameOf(extraArgs))
            End If
            Return Matches(ModeOf(extraArgs(3)), x, extraArgs(0), extraArgs(2))
        End Function

    End Class

End Namespace
