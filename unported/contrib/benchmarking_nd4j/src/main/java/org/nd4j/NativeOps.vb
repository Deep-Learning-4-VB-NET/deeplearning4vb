Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports ScalarMultiplication = org.nd4j.linalg.api.ops.impl.scalar.ScalarMultiplication
Imports Exp = org.nd4j.linalg.api.ops.impl.transforms.strict.Exp
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports org.openjdk.jmh.annotations

Namespace org.nd4j


	Public Class NativeOps

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @State(Scope.Thread) public static class SetupState
		Public Class SetupState
			Friend array As INDArray = Nd4j.ones(1024, 1024)
			Friend arrayRow As INDArray = Nd4j.linspace(1, 1024, 1024)
			Friend arrayColumn As INDArray = Nd4j.linspace(1, 1024, 1024).reshape(ChrW(1024), 1)
			Friend array1 As INDArray = Nd4j.linspace(1, 20480, 20480)
			Friend array2 As INDArray = Nd4j.linspace(1, 20480, 20480)

			Friend array3 As INDArray = Nd4j.ones(128, 256)
			Friend arrayRow3 As INDArray = Nd4j.linspace(1, 256, 256)

			Friend arrayUnordered As INDArray = Nd4j.ones(512, 512)
			Friend arrayOrderedC As INDArray = Nd4j.zeros(512, 512,"c"c)
			Friend arrayOrderedF As INDArray = Nd4j.zeros(512, 512, "f"c)

		End Class


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Benchmark @BenchmarkMode(Mode.AverageTime) @OutputTimeUnit(java.util.concurrent.TimeUnit.MICROSECONDS) public void broadcastColumn(SetupState state) throws java.io.IOException
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub broadcastColumn(ByVal state As SetupState)
			state.array.addiColumnVector(state.arrayColumn)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Benchmark @BenchmarkMode(Mode.AverageTime) @OutputTimeUnit(java.util.concurrent.TimeUnit.MICROSECONDS) public void broadcastRow(SetupState state) throws java.io.IOException
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub broadcastRow(ByVal state As SetupState)
			state.array.addiRowVector(state.arrayRow)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Benchmark @BenchmarkMode(Mode.AverageTime) @OutputTimeUnit(java.util.concurrent.TimeUnit.MICROSECONDS) public void transformOp(SetupState state) throws java.io.IOException
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub transformOp(ByVal state As SetupState)
			Nd4j.Executioner.exec(New Exp(state.array1, state.array2))
		End Sub



'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Benchmark @BenchmarkMode(Mode.AverageTime) @OutputTimeUnit(java.util.concurrent.TimeUnit.MICROSECONDS) public void scalarOp2(SetupState state) throws java.io.IOException
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub scalarOp2(ByVal state As SetupState)
			Nd4j.Executioner.exec(New ScalarMultiplication(state.arrayUnordered, 2.5f))
		End Sub




'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Benchmark @BenchmarkMode(Mode.AverageTime) @OutputTimeUnit(java.util.concurrent.TimeUnit.MICROSECONDS) public void dupDifferentOrdersOp(SetupState state) throws java.io.IOException
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub dupDifferentOrdersOp(ByVal state As SetupState)
			state.arrayUnordered.assign(state.arrayOrderedF)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Benchmark @BenchmarkMode(Mode.AverageTime) @OutputTimeUnit(java.util.concurrent.TimeUnit.MICROSECONDS) public void dupSameOrdersOp(SetupState state) throws java.io.IOException
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub dupSameOrdersOp(ByVal state As SetupState)
			state.arrayUnordered.assign(state.arrayOrderedC)
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Benchmark @BenchmarkMode(Mode.AverageTime) @OutputTimeUnit(java.util.concurrent.TimeUnit.MICROSECONDS) public void pairwiseOp1(SetupState state) throws java.io.IOException
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub pairwiseOp1(ByVal state As SetupState)
			state.array1.addiRowVector(state.array2)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Benchmark @BenchmarkMode(Mode.AverageTime) @OutputTimeUnit(java.util.concurrent.TimeUnit.MICROSECONDS) public void broadcastOp2(SetupState state) throws java.io.IOException
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub broadcastOp2(ByVal state As SetupState)
			state.array.addiRowVector(state.arrayRow)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Benchmark @BenchmarkMode(Mode.AverageTime) @OutputTimeUnit(java.util.concurrent.TimeUnit.MICROSECONDS) public void reduceOp1(SetupState state) throws java.io.IOException
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub reduceOp1(ByVal state As SetupState)
			state.array.sum(0)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Benchmark @BenchmarkMode(Mode.AverageTime) @OutputTimeUnit(java.util.concurrent.TimeUnit.MICROSECONDS) public void reduceOp2(SetupState state) throws java.io.IOException
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub reduceOp2(ByVal state As SetupState)
			state.array.sumNumber().floatValue()
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Benchmark @BenchmarkMode(Mode.AverageTime) @OutputTimeUnit(java.util.concurrent.TimeUnit.MICROSECONDS) public void scalarOp1(SetupState state) throws java.io.IOException
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub scalarOp1(ByVal state As SetupState)
			state.array2.addi(0.5f)
		End Sub


	End Class

End Namespace