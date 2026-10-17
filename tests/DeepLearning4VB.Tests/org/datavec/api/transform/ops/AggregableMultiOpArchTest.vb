Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports AggregatorImpls = DeepLearning4VB.org.datavec.api.transform.ops.AggregatorImpls

Namespace org.datavec.api.transform.ops

    ''' <summary>
    ''' Port of the DataVec <c>AggregableMultiOpArchTest</c>. The Java test is an ArchUnit rule ("all aggregate ops must be
    ''' serializable": every class of the <c>ops</c> package except <c>AggregatorImpls</c>, <c>IAggregableReduceOp</c>,
    ''' <c>StringAggregatorImpls</c> and the compiler-generated anonymous class implements <c>Serializable</c>); here the
    ''' same rule is checked by reflection over the library assembly, with <c>&lt;Serializable&gt;</c> in place of the
    ''' Java interface.
    ''' </summary>
    <TestClass>
    Public Class AggregableMultiOpArchTest

        Private Const OpsNamespace As String = "DeepLearning4VB.org.datavec.api.transform.ops"

        <TestMethod>
        Public Sub allAggregateOpsMustBeSerializable()
            Dim excluded() As String = {"AggregatorImpls", "IAggregableReduceOp", "StringAggregatorImpls"}
            Dim offenders As New List(Of String)()
            Dim checkedTypes As New List(Of String)()
            For Each t As Type In GetType(AggregatorImpls).Assembly.GetTypes()
                If t.Namespace <> OpsNamespace Then Continue For
                ' compiler-generated closures play the role of Java's StringAggregatorImpls$1
                If t.IsDefined(GetType(CompilerGeneratedAttribute), False) OrElse t.Name.Contains("$") Then Continue For
                Dim simpleName As String = t.Name
                Dim tick As Integer = simpleName.IndexOf("`"c)
                If tick >= 0 Then simpleName = simpleName.Substring(0, tick)
                If excluded.Contains(simpleName) Then Continue For
                checkedTypes.Add(t.FullName)
                If Not t.IsSerializable Then offenders.Add(t.FullName)
            Next t

            ' The rule must actually cover the aggregators (AggregableFirst ... AggregableCountUnique, the
            ' string reducers, the writable ops and the dispatch/multi ops).
            assertTrue(checkedTypes.Count >= 20, "only " & checkedTypes.Count & " types checked")
            assertTrue(checkedTypes.Any(Function(n) n.EndsWith("+AggregableSum`1", StringComparison.Ordinal)))
            assertTrue(checkedTypes.Any(Function(n) n.EndsWith("+AggregableStringAppend", StringComparison.Ordinal)))
            assertTrue(checkedTypes.Any(Function(n) n.EndsWith(".AggregableMultiOp`1", StringComparison.Ordinal)))
            assertEquals(0, offenders.Count, "All aggregate ops must be serializable: " & String.Join(", ", offenders))
        End Sub

    End Class

End Namespace
