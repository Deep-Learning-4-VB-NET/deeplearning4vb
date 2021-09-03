Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports org.openjdk.jmh.annotations

Namespace org.nd4j


	Public Class BlasWrapper

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @State(Scope.Thread) public static class SetupState
		Public Class SetupState
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: public org.nd4j.linalg.api.ndarray.INDArray array1 = org.nd4j.linalg.factory.Nd4j.ones(100).addi(0.01f) public org.nd4j.linalg.api.ndarray.INDArray array2 = org.nd4j.linalg.factory.Nd4j.ones(100).addi(0.01f) public org.nd4j.linalg.api.ndarray.INDArray array3 = org.nd4j.linalg.factory.Nd4j.ones(100).addi(0.01f) public org.nd4j.linalg.factory.BlasWrapper wrapper = org.nd4j.linalg.factory.Nd4j.getBlasWrapper();
			Nd4j.ones(100).addi(0.01f) public org.nd4j.linalg.factory.BlasWrapper wrapper = Nd4j.BlasWrapper
				Public As As array1

		End Class

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Benchmark @BenchmarkMode(Mode.AverageTime) @OutputTimeUnit(java.util.concurrent.TimeUnit.NANOSECONDS) public void asum(SetupState state)
		Public Overridable Sub asum(ByVal state As SetupState)
			state.wrapper.asum(state.array1)
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Benchmark @BenchmarkMode(Mode.AverageTime) @OutputTimeUnit(java.util.concurrent.TimeUnit.NANOSECONDS) public void axpy(SetupState state)
		Public Overridable Sub axpy(ByVal state As SetupState)
		   state.wrapper.axpy(New Single?(0.75f), state.array1, state.array2)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Benchmark @BenchmarkMode(Mode.AverageTime) @OutputTimeUnit(java.util.concurrent.TimeUnit.NANOSECONDS) public void copy(SetupState state)
		Public Overridable Sub copy(ByVal state As SetupState)
			state.wrapper.copy(state.array1, state.array2)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Benchmark @BenchmarkMode(Mode.AverageTime) @OutputTimeUnit(java.util.concurrent.TimeUnit.NANOSECONDS) public void dot(SetupState state)
		Public Overridable Sub dot(ByVal state As SetupState)
			state.wrapper.dot(state.array1, state.array2)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Benchmark @BenchmarkMode(Mode.AverageTime) @OutputTimeUnit(java.util.concurrent.TimeUnit.NANOSECONDS) public void nrm2(SetupState state)
		Public Overridable Sub nrm2(ByVal state As SetupState)
			state.wrapper.nrm2(state.array1)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Benchmark @BenchmarkMode(Mode.AverageTime) @OutputTimeUnit(java.util.concurrent.TimeUnit.NANOSECONDS) public void iamax(SetupState state)
		Public Overridable Sub iamax(ByVal state As SetupState)
			state.wrapper.iamax(state.array1)
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Benchmark @BenchmarkMode(Mode.AverageTime) @OutputTimeUnit(java.util.concurrent.TimeUnit.NANOSECONDS) public void swap(SetupState state)
		Public Overridable Sub swap(ByVal state As SetupState)
			state.wrapper.swap(state.array1, state.array2)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Benchmark @BenchmarkMode(Mode.AverageTime) @OutputTimeUnit(java.util.concurrent.TimeUnit.NANOSECONDS) public void gemv(SetupState state)
		Public Overridable Sub gemv(ByVal state As SetupState)
			state.wrapper.gemv(CType(New Single?(0.75f), Number), state.array1, state.array2, New Double?(0.5), state.array3)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Benchmark @BenchmarkMode(Mode.AverageTime) @OutputTimeUnit(java.util.concurrent.TimeUnit.NANOSECONDS) public void ger(SetupState state)
		Public Overridable Sub ger(ByVal state As SetupState)
			state.wrapper.ger(New Double?(0.75f), state.array1, state.array2, state.array3)
		End Sub
	End Class

End Namespace