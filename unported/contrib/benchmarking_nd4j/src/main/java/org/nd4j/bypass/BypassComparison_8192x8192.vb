Imports System
Imports FloatPointer = org.bytedeco.javacpp.FloatPointer
Imports openblas = org.bytedeco.openblas.global.openblas
Imports Level3 = org.nd4j.linalg.api.blas.Level3
Imports GemmParams = org.nd4j.linalg.api.blas.params.GemmParams
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports org.openjdk.jmh.annotations
Imports Blackhole = org.openjdk.jmh.infra.Blackhole

Namespace org.nd4j.bypass


	Public Class BypassComparison_8192x8192


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @State(Scope.Thread) public static class SetupState
		Public Class SetupState
			Public size As Integer = 8192
			Public m1 As INDArray = Nd4j.ones(size, size)
			Public m2 As INDArray = Nd4j.ones(m1.shape())
			Public r As INDArray = Nd4j.createUninitialized(m1.shape(), "f"c)

			Public wrapper As Level3 = Nd4j.BlasWrapper.level3()
			Public sgemm As System.Reflection.MethodInfo
			Public params As New GemmParams(m1, m2, r)

			Friend a As FloatPointer = CType(params.getA().data().addressPointer(), FloatPointer)
			Friend b As FloatPointer = CType(params.getB().data().addressPointer(), FloatPointer)
			Friend c As FloatPointer = CType(params.getC().data().addressPointer(), FloatPointer)

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Setup(Level.Trial) public void doSetup()
			Public Overridable Sub doSetup()
				Try
					sgemm = wrapper.GetType().getDeclaredMethod("sgemm", GetType(Char), GetType(Char), GetType(Char), GetType(Integer), GetType(Integer), GetType(Integer), GetType(Single), GetType(INDArray), GetType(Integer), GetType(INDArray), GetType(Integer), GetType(Single), GetType(INDArray), GetType(Integer))
					sgemm.setAccessible(True)
				Catch e As NoSuchMethodException
					Console.WriteLine(e.ToString())
					Console.Write(e.StackTrace)
				End Try
			End Sub
		End Class


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Benchmark @BenchmarkMode(Mode.AverageTime) @OutputTimeUnit(java.util.concurrent.TimeUnit.SECONDS) public void gemm(SetupState state, org.openjdk.jmh.infra.Blackhole bh)
		Public Overridable Sub gemm(ByVal state As SetupState, ByVal bh As Blackhole)
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.nd4j.linalg.api.blas.params.GemmParams params = state.params;
			Dim params As GemmParams = state.params
			Try
				state.sgemm.invoke(state.wrapper, params.getA().ordering(), params.getTransA(), params.getTransB(), params.getM(), params.getN(), params.getK(), CSng(1.0), params.getA(), params.getLda(), params.getB(), params.getLdb(), CSng(0.0), params.getC(), params.getLdc())
			Catch e As IllegalAccessException
				Console.WriteLine(e.ToString())
				Console.Write(e.StackTrace)
			Catch e As InvocationTargetException
				Console.WriteLine(e.ToString())
				Console.Write(e.StackTrace)
			End Try
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Benchmark @BenchmarkMode(Mode.AverageTime) @OutputTimeUnit(java.util.concurrent.TimeUnit.SECONDS) public void cblas_gemm(SetupState state, org.openjdk.jmh.infra.Blackhole bh)
		Public Overridable Sub cblas_gemm(ByVal state As SetupState, ByVal bh As Blackhole)
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.nd4j.linalg.api.blas.params.GemmParams params = state.params;
			Dim params As GemmParams = state.params
			openblas.cblas_sgemm(102,111, 111, params.getM(), params.getN(), params.getK(), 1.0f, state.a, params.getLda(), state.b, params.getLdb(), 0.0f, state.c, params.getLdc())
		End Sub




	End Class

End Namespace