Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports org.openjdk.jmh.annotations

Namespace org.nd4j


	Public Class Flattening

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @State(Scope.Thread) public static class SetupState
		Public Class SetupState
			Public small_c As INDArray = org.nd4j.linalg.factory.Nd4j.create(New Integer(){1<<10, 1<<10}, "c"c)
			Public small_f As INDArray = org.nd4j.linalg.factory.Nd4j.create(New Integer(){1<<10, 1<<10}, "f"c)
		End Class

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Benchmark @BenchmarkMode(Mode.AverageTime) @OutputTimeUnit(java.util.concurrent.TimeUnit.MICROSECONDS) public void toFlattened_CC_Small(SetupState state) throws java.io.IOException
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub toFlattened_CC_Small(ByVal state As SetupState)
			org.nd4j.linalg.factory.Nd4j.toFlattened("c"c, state.small_c)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Benchmark @BenchmarkMode(Mode.AverageTime) @OutputTimeUnit(java.util.concurrent.TimeUnit.MICROSECONDS) public void toFlattened_CF_Small(SetupState state) throws java.io.IOException
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub toFlattened_CF_Small(ByVal state As SetupState)
			org.nd4j.linalg.factory.Nd4j.toFlattened("f"c, state.small_c)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Benchmark @BenchmarkMode(Mode.AverageTime) @OutputTimeUnit(java.util.concurrent.TimeUnit.MICROSECONDS) public void toFlattened_FF_Small(SetupState state) throws java.io.IOException
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub toFlattened_FF_Small(ByVal state As SetupState)
			org.nd4j.linalg.factory.Nd4j.toFlattened("f"c, state.small_f)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Benchmark @BenchmarkMode(Mode.AverageTime) @OutputTimeUnit(java.util.concurrent.TimeUnit.MICROSECONDS) public void toFlattened_FC_Small(SetupState state) throws java.io.IOException
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub toFlattened_FC_Small(ByVal state As SetupState)
			org.nd4j.linalg.factory.Nd4j.toFlattened("c"c, state.small_f)
		End Sub

	End Class

End Namespace