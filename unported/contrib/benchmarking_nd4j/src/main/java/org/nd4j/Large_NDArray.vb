Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports org.openjdk.jmh.annotations

Namespace org.nd4j


	Public Class Large_NDArray

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @State(Scope.Thread) public static class SetupState
		Public Class SetupState
			Public array1 As INDArray = Nd4j.ones(1<<28)
			Public array2 As INDArray = Nd4j.ones(1<<28)

			Shared Sub New()
				' Only needed for mkl on RC3.8
				'System.loadLibrary("mkl_rt");
			End Sub
		End Class


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Benchmark @BenchmarkMode(Mode.AverageTime) @OutputTimeUnit(java.util.concurrent.TimeUnit.MILLISECONDS) public void sumNumber(SetupState state)
		Public Overridable Sub sumNumber(ByVal state As SetupState)
			state.array1.sumNumber().doubleValue()
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Benchmark @BenchmarkMode(Mode.AverageTime) @OutputTimeUnit(java.util.concurrent.TimeUnit.MILLISECONDS) public void add(SetupState state)
		Public Overridable Sub add(ByVal state As SetupState)
			state.array1.add(state.array2)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Benchmark @BenchmarkMode(Mode.AverageTime) @OutputTimeUnit(java.util.concurrent.TimeUnit.MILLISECONDS) public void addi(SetupState state)
		Public Overridable Sub addi(ByVal state As SetupState)
			state.array1.addi(state.array2)
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Benchmark @BenchmarkMode(Mode.AverageTime) @OutputTimeUnit(java.util.concurrent.TimeUnit.MILLISECONDS) public void sub(SetupState state)
		Public Overridable Sub [sub](ByVal state As SetupState)
			state.array1.sub(state.array2)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Benchmark @BenchmarkMode(Mode.AverageTime) @OutputTimeUnit(java.util.concurrent.TimeUnit.MILLISECONDS) public void subi(SetupState state)
		Public Overridable Sub subi(ByVal state As SetupState)
			state.array1.subi(state.array2)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Benchmark @BenchmarkMode(Mode.AverageTime) @OutputTimeUnit(java.util.concurrent.TimeUnit.MILLISECONDS) public void mul(SetupState state)
		Public Overridable Sub mul(ByVal state As SetupState)
			state.array1.mul(state.array2)
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Benchmark @BenchmarkMode(Mode.AverageTime) @OutputTimeUnit(java.util.concurrent.TimeUnit.MILLISECONDS) public void muli(SetupState state)
		Public Overridable Sub muli(ByVal state As SetupState)
			state.array1.muli(state.array2)
		End Sub

	'    @Benchmark @BenchmarkMode(Mode.AverageTime) @OutputTimeUnit(TimeUnit.MILLISECONDS)
	'    public void cumsum(SetupState state) {
	'        state.array1.cumsum(0);
	'    }
	'
	'
	'    @Benchmark @BenchmarkMode(Mode.AverageTime) @OutputTimeUnit(TimeUnit.MILLISECONDS)
	'    public void cumsumi(SetupState state) {
	'        state.array1.cumsumi(0);
	'    }

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Benchmark @BenchmarkMode(Mode.AverageTime) @OutputTimeUnit(java.util.concurrent.TimeUnit.MILLISECONDS) public void assign(SetupState state)
		Public Overridable Sub assign(ByVal state As SetupState)
			state.array1.assign(state.array2)
		End Sub

	End Class

End Namespace