Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports NativeOpsHolder = org.nd4j.nativeblas.NativeOpsHolder
Imports org.openjdk.jmh.annotations

Namespace org.nd4j


	Public Class Small_NDArray


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @State(Scope.Thread) public static class SetupState
		Public Class SetupState
			Public array1 As INDArray = Nd4j.ones(1<<8)
			Public array2 As INDArray = Nd4j.ones(1<<8)

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Setup public void setup()
			Public Overridable Sub setup()
				NativeOpsHolder.Instance.getDeviceNativeOps().setElementThreshold(16384)
				NativeOpsHolder.Instance.getDeviceNativeOps().setTADThreshold(64)
			End Sub
		End Class


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Benchmark @BenchmarkMode(Mode.AverageTime) @OutputTimeUnit(java.util.concurrent.TimeUnit.NANOSECONDS) public void sumNumber(SetupState state)
		Public Overridable Sub sumNumber(ByVal state As SetupState)
			state.array1.sumNumber().doubleValue()
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Benchmark @BenchmarkMode(Mode.AverageTime) @OutputTimeUnit(java.util.concurrent.TimeUnit.NANOSECONDS) public void add(SetupState state)
		Public Overridable Sub add(ByVal state As SetupState)
			state.array1.add(state.array2)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Benchmark @BenchmarkMode(Mode.AverageTime) @OutputTimeUnit(java.util.concurrent.TimeUnit.NANOSECONDS) public void addi(SetupState state)
		Public Overridable Sub addi(ByVal state As SetupState)
			state.array1.addi(state.array2)
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Benchmark @BenchmarkMode(Mode.AverageTime) @OutputTimeUnit(java.util.concurrent.TimeUnit.NANOSECONDS) public void sub(SetupState state)
		Public Overridable Sub [sub](ByVal state As SetupState)
			state.array1.sub(state.array2)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Benchmark @BenchmarkMode(Mode.AverageTime) @OutputTimeUnit(java.util.concurrent.TimeUnit.NANOSECONDS) public void subi(SetupState state)
		Public Overridable Sub subi(ByVal state As SetupState)
			state.array1.subi(state.array2)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Benchmark @BenchmarkMode(Mode.AverageTime) @OutputTimeUnit(java.util.concurrent.TimeUnit.NANOSECONDS) public void mul(SetupState state)
		Public Overridable Sub mul(ByVal state As SetupState)
			state.array1.mul(state.array2)
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Benchmark @BenchmarkMode(Mode.AverageTime) @OutputTimeUnit(java.util.concurrent.TimeUnit.NANOSECONDS) public void muli(SetupState state)
		Public Overridable Sub muli(ByVal state As SetupState)
			state.array1.muli(state.array2)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Benchmark @BenchmarkMode(Mode.AverageTime) @OutputTimeUnit(java.util.concurrent.TimeUnit.NANOSECONDS) public void cumsum(SetupState state)
		Public Overridable Sub cumsum(ByVal state As SetupState)
			state.array1.cumsum(0)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Benchmark @BenchmarkMode(Mode.AverageTime) @OutputTimeUnit(java.util.concurrent.TimeUnit.NANOSECONDS) public void cumsumi(SetupState state)
		Public Overridable Sub cumsumi(ByVal state As SetupState)
			state.array1.cumsumi(0)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Benchmark @BenchmarkMode(Mode.AverageTime) @OutputTimeUnit(java.util.concurrent.TimeUnit.NANOSECONDS) public void assign(SetupState state)
		Public Overridable Sub assign(ByVal state As SetupState)
			state.array1.assign(state.array2)
		End Sub

	End Class

End Namespace