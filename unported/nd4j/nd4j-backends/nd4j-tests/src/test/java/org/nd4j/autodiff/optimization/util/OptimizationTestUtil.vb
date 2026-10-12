Imports System
Imports System.Collections.Generic
Imports SDVariable = org.nd4j.autodiff.samediff.SDVariable
Imports SameDiff = org.nd4j.autodiff.samediff.SameDiff
Imports VariableType = org.nd4j.autodiff.samediff.VariableType
Imports GraphOptimizer = org.nd4j.autodiff.samediff.optimize.GraphOptimizer
Imports Optimizer = org.nd4j.autodiff.samediff.optimize.Optimizer
Imports OptimizerSet = org.nd4j.autodiff.samediff.optimize.OptimizerSet
Imports Preconditions = org.nd4j.common.base.Preconditions
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
import static org.junit.Assert.assertEquals
import static org.junit.Assert.assertTrue

Namespace org.nd4j.autodiff.optimization.util


	''' <summary>
	''' TODO:
	''' - Add ability to track which optimization functions exactly were applied!
	''' </summary>
	Public Class OptimizationTestUtil

		Private Sub New()
		End Sub

		Public Shared Function testOptimization(ByVal config As OptTestConfig) As SameDiff
			Preconditions.checkNotNull(config.getTempFolder(), "Temp folder should be specified before running test")

			Dim optimizerSets As IList(Of OptimizerSet) = config.getOptimizerSets()
			If optimizerSets Is Nothing Then
				optimizerSets = GraphOptimizer.defaultOptimizations()
			End If
			Dim debugger As New OptimizationRecordingDebugger()

			'
			Dim ph As IDictionary(Of String, INDArray) = config.getPlaceholders()
			Dim outputs As IList(Of String) = config.getOutputs()
			Dim original As SameDiff = config.getOriginal()
			Dim copy As SameDiff = original.dup()
			Dim optimized As SameDiff = GraphOptimizer.optimize(original, outputs, optimizerSets, debugger)

			'Check that SOMETHING changed in the optimized - number of constants, variables, or ops; or the settings for ops; or the values of some arrays
			'TODO
			Dim sameNumConst As Boolean = original.getConstantArrays().size() = optimized.getConstantArrays().size()
			Dim sameNumVars As Boolean = original.getVariablesArrays().size() = optimized.getVariablesArrays().size()
			Dim sameNumSDVars As Boolean = original.getVariables().size() = optimized.getVariables().size()
			Dim sameNumOps As Boolean = original.getOps().size() = optimized.getOps().size()

			If sameNumConst AndAlso sameNumVars AndAlso sameNumSDVars AndAlso sameNumOps Then


				Throw New System.InvalidOperationException("Did not detect any changes to the graph structure after optimization (but check is AS YET WIP)")
			End If

			'Check that optimizations we expected to be applied were in fact applied:
			Dim mustApply As IDictionary(Of String, Type) = config.getMustApply()
			Dim applied As IDictionary(Of String, Optimizer) = debugger.getApplied()
			For Each s As String In mustApply.Keys
				assertTrue("Expected optimizer of type " & mustApply(s).Name & " to be applied to op " & s, applied.ContainsKey(s))
			Next s


			'Second: check that they all produce the same
			'TODO this won't work for random ops!
			Dim origOut As IDictionary(Of String, INDArray) = original.output(ph, outputs)
			Dim copyOut As IDictionary(Of String, INDArray) = copy.output(ph, outputs)
			Dim optimizedOut As IDictionary(Of String, INDArray) = optimized.output(ph, outputs)

			assertEquals(copyOut, origOut)
			assertEquals(copyOut, optimizedOut)

			Dim f As New File(config.getTempFolder(), "optimized.sd")
			optimized.save(f, True)

			Dim loaded As SameDiff = SameDiff.load(f, True)
			Dim loadedOut As IDictionary(Of String, INDArray) = loaded.output(ph, outputs)
			assertEquals(copyOut, loadedOut)

			'TODO add support for training checks!
			'This is especially important for updaters... if we permute the weights, we should permute the updater state also

			'Check that nothing has changed (from the user API perspective) for the original graph
			'i.e.,
			For Each v As SDVariable In copy.variables()
				Dim ov As SDVariable = original.getVariable(v.name())

				assertEquals(v.dataType(), ov.dataType())
				assertEquals(v.getVariableType(), ov.getVariableType())
				If v.getVariableType() = VariableType.CONSTANT OrElse v.getVariableType() = VariableType.VARIABLE Then
					Dim arrCopy As INDArray = v.Arr
					Dim arrOrig As INDArray = ov.Arr
					assertEquals(arrCopy, arrOrig)
				End If

			Next v

			Return optimized
		End Function

	End Class
End Namespace