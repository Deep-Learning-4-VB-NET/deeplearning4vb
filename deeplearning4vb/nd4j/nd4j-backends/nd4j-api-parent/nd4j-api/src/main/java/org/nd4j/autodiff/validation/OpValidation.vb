Imports System
Imports System.Collections.Generic
Imports Microsoft.VisualBasic
Imports ND4JClassLoading = org.nd4j.common.config.ND4JClassLoading
Imports org.nd4j.linalg.api.ops.custom
Imports ArgMax = org.nd4j.linalg.api.ops.impl.indexaccum.custom.ArgMax
Imports ArgMin = org.nd4j.linalg.api.ops.impl.indexaccum.custom.ArgMin
Imports HashCode = org.nd4j.linalg.api.ops.impl.reduce.HashCode
Imports ImmutableSet = org.nd4j.shade.guava.collect.ImmutableSet
Imports ClassPath = org.nd4j.shade.guava.reflect.ClassPath
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports val = lombok.val
Imports DifferentialFunction = org.nd4j.autodiff.functions.DifferentialFunction
Imports Listener = org.nd4j.autodiff.listeners.Listener
Imports SDVariable = org.nd4j.autodiff.samediff.SDVariable
Imports SameDiff = org.nd4j.autodiff.samediff.SameDiff
Imports SameDiffOp = org.nd4j.autodiff.samediff.internal.SameDiffOp
Imports Variable = org.nd4j.autodiff.samediff.internal.Variable
Imports NonInplaceValidationListener = org.nd4j.autodiff.validation.listeners.NonInplaceValidationListener
Imports Preconditions = org.nd4j.common.base.Preconditions
Imports DifferentialFunctionClassHolder = org.nd4j.imports.converters.DifferentialFunctionClassHolder
Imports TensorflowDescriptorParser = org.nd4j.imports.descriptors.tensorflow.TensorflowDescriptorParser
Imports NdIndexIterator = org.nd4j.linalg.api.iter.NdIndexIterator
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports CustomOpDescriptor = org.nd4j.linalg.api.ops.CustomOpDescriptor
Imports DynamicCustomOp = org.nd4j.linalg.api.ops.DynamicCustomOp
Imports org.nd4j.linalg.api.ops.impl.broadcast.bool
Imports ExternalErrorsFunction = org.nd4j.linalg.api.ops.impl.layers.ExternalErrorsFunction
Imports org.nd4j.linalg.api.ops.impl.loss.bp
Imports InvertedPredicateMetaOp = org.nd4j.linalg.api.ops.impl.meta.InvertedPredicateMetaOp
Imports PostulateMetaOp = org.nd4j.linalg.api.ops.impl.meta.PostulateMetaOp
Imports PredicateMetaOp = org.nd4j.linalg.api.ops.impl.meta.PredicateMetaOp
Imports ReduceMetaOp = org.nd4j.linalg.api.ops.impl.meta.ReduceMetaOp
Imports CbowRound = org.nd4j.linalg.api.ops.impl.nlp.CbowRound
Imports SkipGramRound = org.nd4j.linalg.api.ops.impl.nlp.SkipGramRound
Imports MmulBp = org.nd4j.linalg.api.ops.impl.reduce.MmulBp
Imports All = org.nd4j.linalg.api.ops.impl.reduce.bool.All
Imports Any = org.nd4j.linalg.api.ops.impl.reduce.bool.Any
Imports IsInf = org.nd4j.linalg.api.ops.impl.reduce.bool.IsInf
Imports IsNaN = org.nd4j.linalg.api.ops.impl.reduce.bool.IsNaN
Imports MatchCondition = org.nd4j.linalg.api.ops.impl.reduce.longer.MatchCondition
Imports EqualsWithEps = org.nd4j.linalg.api.ops.impl.reduce3.EqualsWithEps
Imports NormalizeMoments = org.nd4j.linalg.api.ops.impl.reduce.NormalizeMoments
Imports org.nd4j.linalg.api.ops.impl.reduce.bp
Imports org.nd4j.linalg.api.ops.impl.broadcast
Imports FreeGridOp = org.nd4j.linalg.api.ops.impl.grid.FreeGridOp
Imports org.nd4j.linalg.api.ops.impl.indexaccum
Imports org.nd4j.linalg.api.ops.impl.layers.convolution
Imports PowDerivative = org.nd4j.linalg.api.ops.impl.scalar.PowDerivative
Imports ScalarRemainder = org.nd4j.linalg.api.ops.impl.scalar.ScalarRemainder
Imports ScalarSetValue = org.nd4j.linalg.api.ops.impl.scalar.comparison.ScalarSetValue
Imports org.nd4j.linalg.api.ops.impl.shape
Imports ConcatBp = org.nd4j.linalg.api.ops.impl.shape.bp.ConcatBp
Imports SliceBp = org.nd4j.linalg.api.ops.impl.shape.bp.SliceBp
Imports StridedSliceBp = org.nd4j.linalg.api.ops.impl.shape.bp.StridedSliceBp
Imports TileBp = org.nd4j.linalg.api.ops.impl.shape.bp.TileBp
Imports Assert = org.nd4j.linalg.api.ops.impl.transforms.Assert
Imports Histogram = org.nd4j.linalg.api.ops.impl.transforms.Histogram
Imports BooleanNot = org.nd4j.linalg.api.ops.impl.transforms.bool.BooleanNot
Imports MatchConditionTransform = org.nd4j.linalg.api.ops.impl.transforms.bool.MatchConditionTransform
Imports org.nd4j.linalg.api.ops.impl.transforms.custom
Imports BinaryMinimalRelativeError = org.nd4j.linalg.api.ops.impl.transforms.pairwise.BinaryMinimalRelativeError
Imports org.nd4j.linalg.api.ops.impl.transforms.pairwise.arithmetic.bp
Imports org.nd4j.linalg.api.ops.impl.transforms.gradient
Imports [Not] = org.nd4j.linalg.api.ops.impl.transforms.pairwise.bool.Not
Imports org.nd4j.linalg.api.ops.impl.transforms.segment.bp
Imports GELUDerivative = org.nd4j.linalg.api.ops.impl.transforms.strict.GELUDerivative
Imports PreciseGELUDerivative = org.nd4j.linalg.api.ops.impl.transforms.strict.PreciseGELUDerivative
Imports SwishDerivative = org.nd4j.linalg.api.ops.impl.transforms.strict.SwishDerivative
Imports TanDerivative = org.nd4j.linalg.api.ops.impl.transforms.strict.TanDerivative
Imports RestoreV2 = org.nd4j.linalg.api.ops.persistence.RestoreV2
Imports SaveV2 = org.nd4j.linalg.api.ops.persistence.SaveV2
Imports RandomStandardNormal = org.nd4j.linalg.api.ops.random.compat.RandomStandardNormal
Imports DistributionUniform = org.nd4j.linalg.api.ops.random.custom.DistributionUniform
Imports org.nd4j.linalg.api.ops.random.impl
Imports Linspace = org.nd4j.linalg.api.ops.random.impl.Linspace
Imports LongShapeDescriptor = org.nd4j.linalg.api.shape.LongShapeDescriptor
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports org.nd4j.common.function
Imports Conditions = org.nd4j.linalg.indexing.conditions.Conditions
Imports org.nd4j.common.primitives
Imports OpDef = org.tensorflow.framework.OpDef

'
' *  ******************************************************************************
' *  *
' *  *
' *  * This program and the accompanying materials are made available under the
' *  * terms of the Apache License, Version 2.0 which is available at
' *  * https://www.apache.org/licenses/LICENSE-2.0.
' *  *
' *  *  See the NOTICE file distributed with this work for additional
' *  *  information regarding copyright ownership.
' *  * Unless required by applicable law or agreed to in writing, software
' *  * distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
' *  * WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the
' *  * License for the specific language governing permissions and limitations
' *  * under the License.
' *  *
' *  * SPDX-License-Identifier: Apache-2.0
' *  *****************************************************************************
' 

Namespace org.nd4j.autodiff.validation


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j public class OpValidation
	Public Class OpValidation

		''' <summary>
		''' Run test case
		''' </summary>
		''' <param name="testCase"> Test case to run </param>
		''' <returns> NULL if test passes, or error message otherwise </returns>
'JAVA TO VB CONVERTER NOTE: The parameter testCase was renamed since it may cause conflicts with calls to static members of the user-defined type with this name:
		Public Shared Function validate(ByVal testCase_Conflict As TestCase) As String
			Return validate(testCase_Conflict, False)
		End Function

'JAVA TO VB CONVERTER NOTE: The parameter testCase was renamed since it may cause conflicts with calls to static members of the user-defined type with this name:
		Public Shared Function validate(ByVal testCase_Conflict As TestCase, ByVal exceptionsAsErrorMsg As Boolean) As String
			Try
				Return validateHelper(testCase_Conflict)
			Catch t As Exception
				If exceptionsAsErrorMsg Then
					log.info("Exception encountered - returning as error message", t)
					Return "EXCEPTION: " & t.getMessage()
				End If
				Throw t
			End Try
		End Function

'JAVA TO VB CONVERTER NOTE: The parameter testCase was renamed since it may cause conflicts with calls to static members of the user-defined type with this name:
		Private Shared Function validateHelper(ByVal testCase_Conflict As TestCase) As String
			testCase_Conflict.assertConfigValid()

			'First: collect coverage information
			collectCoverageInformation(testCase_Conflict)

			'Check serialization
			Dim serializedBeforeExec As ByteBuffer = Nothing
			If testCase_Conflict.testFlatBufferSerialization() = TestCase.TestSerialization.BEFORE_EXEC OrElse testCase_Conflict.testFlatBufferSerialization() = TestCase.TestSerialization.BOTH Then
				serializedBeforeExec = testCase_Conflict.sameDiff().asFlatBuffers(True)
				Preconditions.checkNotNull(serializedBeforeExec, "Serialization failed? Null output")
			End If

			Dim sameDiff As SameDiff = testCase_Conflict.sameDiff()
			Dim listeners As IList(Of Listener) = sameDiff.getListeners()
			If listeners.Count = 0 Then
				sameDiff.addListeners(New NonInplaceValidationListener())
			Else
				Dim found As Boolean = False
				For Each l As Listener In listeners
					If TypeOf l Is NonInplaceValidationListener Then
						found = True
						Exit For
					End If
				Next l
				If Not found Then
					sameDiff.addListeners(New NonInplaceValidationListener())
				End If
			End If

			'Check forward pass:
			If testCase_Conflict.fwdTestFns() IsNot Nothing AndAlso testCase_Conflict.fwdTestFns().size() > 0 Then
				Dim sd As SameDiff = testCase_Conflict.sameDiff()

				'Collect variables we need outputs for...
				Dim reqVars As [Set](Of String) = testCase_Conflict.fwdTestFns().keySet()

				Dim [out] As IDictionary(Of String, INDArray)
				Try
					[out] = sd.output(testCase_Conflict.placeholderValues(), New List(Of )(reqVars))
				Catch e As Exception
					Throw New Exception("Error during forward pass testing" & testCase_Conflict.testNameErrMsg(), e)
				End Try

				For Each e As KeyValuePair(Of String, [Function](Of INDArray, String)) In testCase_Conflict.fwdTestFns().entrySet()
					Dim v As SDVariable = sd.getVariable(e.Key)
					If v Is Nothing Then
						Throw New System.InvalidOperationException("Test case has expected result function defined for variable """ & e.Key & """ but SameDiff instance does not have a variable for this name" & testCase_Conflict.testNameErrMsg())
					End If

					Dim actual As INDArray = [out](v.name())
					If actual Is Nothing Then
						Throw New System.InvalidOperationException("Null INDArray after forward pass for variable """ & e.Key & """")
					End If

					Dim [error] As String
					Try
						[error] = e.Value.apply(actual)
					Catch t As Exception
						Throw New System.InvalidOperationException("Error checking forward pass for variable """ & e.Key & """: exception was" & " thrown by forward pass validation function", t)
					End Try

					If [error] IsNot Nothing Then
						Return testCase_Conflict.testNameErrMsg() & ": Variable " & e.Key & " failed: " & [error]
					End If
				Next e

				Dim serializedAfterExec As ByteBuffer = Nothing
				If testCase_Conflict.testFlatBufferSerialization() = TestCase.TestSerialization.BEFORE_EXEC OrElse testCase_Conflict.testFlatBufferSerialization() = TestCase.TestSerialization.BOTH Then
					serializedAfterExec = testCase_Conflict.sameDiff().asFlatBuffers(True)
					Preconditions.checkNotNull(serializedAfterExec, "Serialization failed? Null output")
				End If

				'Now: deserialize, and check the results
				If serializedBeforeExec IsNot Nothing Then
					checkDeserializedEquality(sd, serializedBeforeExec, testCase_Conflict)
				End If
			End If

			'Check gradients:
			If testCase_Conflict.gradientCheck() Then
				Dim ok As Boolean
				Try
					ok = GradCheckUtil.checkGradients(testCase_Conflict)
				Catch t As Exception
					t.printStackTrace()
					Throw New System.InvalidOperationException("Exception encountered during gradient check" & testCase_Conflict.testNameErrMsg(), t)
				End Try

				If Not ok Then
					Return "Gradient check failed" & testCase_Conflict.testNameErrMsg()
				End If
			End If

			Return Nothing 'OK - passed
		End Function

		Public Shared Sub checkDeserializedEquality(ByVal original As SameDiff, ByVal bbSerialized As ByteBuffer, ByVal tc As TestCase)
			Dim deserialized As SameDiff
			Try
				deserialized = SameDiff.fromFlatBuffers(bbSerialized)
			Catch e As IOException
				Throw New Exception("IOException deserializing from FlatBuffers", e)
			End Try

			'Check variables:
			Dim vars As IList(Of SDVariable) = original.variables()
			Dim varsDe As IList(Of SDVariable) = deserialized.variables()
			Preconditions.checkState(vars.Count = varsDe.Count, "Number of variables differs: expected %s, got %s", vars.Count, varsDe.Count)
			For i As Integer = 0 To vars.Count - 1
				Dim vO As SDVariable = vars(i)
				Dim vD As SDVariable = varsDe(i)
				Preconditions.checkState(vO.name().Equals(vD.name()), "Names should be equal for variable %s: expected %s vs %s", i, vO.name(), vD.name())
			Next i

			'Check ops:
			Dim opsOrig As IDictionary(Of String, SameDiffOp) = original.getOps()
			Dim opsDeser As IDictionary(Of String, SameDiffOp) = deserialized.getOps()
			Preconditions.checkState(opsOrig.Keys.Equals(opsDeser.Keys), "Op names differs: %s vs. %s", opsOrig.Keys, opsDeser.Keys)

			For Each s As String In opsOrig.Keys
				Dim orig As SameDiffOp = opsOrig(s)
				Dim des As SameDiffOp = opsDeser(s)
				Preconditions.checkState(orig.Name.Equals(des.Name), "Names differ: %s vs %s", orig.Name, des.Name)
				Preconditions.checkState((orig.getInputsToOp() Is Nothing) = (des.getInputsToOp() Is Nothing), "Inputs differ: %s vs. %s", orig.getInputsToOp(), des.getInputsToOp())
'JAVA TO VB CONVERTER WARNING: LINQ 'SequenceEqual' is not always identical to Java AbstractList 'equals':
'ORIGINAL LINE: org.nd4j.common.base.Preconditions.checkState(orig.getInputsToOp() == null || orig.getInputsToOp().equals(des.getInputsToOp()), "Inputs differ: %s vs. %s", orig.getInputsToOp(), des.getInputsToOp());
				Preconditions.checkState(orig.getInputsToOp() Is Nothing OrElse orig.getInputsToOp().SequenceEqual(des.getInputsToOp()), "Inputs differ: %s vs. %s", orig.getInputsToOp(), des.getInputsToOp())

				Preconditions.checkState((orig.getOutputsOfOp() Is Nothing) = (des.getOutputsOfOp() Is Nothing), "Outputs differ: %s vs. %s", orig.getOutputsOfOp(), des.getOutputsOfOp())
'JAVA TO VB CONVERTER WARNING: LINQ 'SequenceEqual' is not always identical to Java AbstractList 'equals':
'ORIGINAL LINE: org.nd4j.common.base.Preconditions.checkState(orig.getOutputsOfOp() == null || orig.getOutputsOfOp().equals(des.getOutputsOfOp()), "Outputs differ: %s vs. %s", orig.getOutputsOfOp(), des.getOutputsOfOp());
				Preconditions.checkState(orig.getOutputsOfOp() Is Nothing OrElse orig.getOutputsOfOp().SequenceEqual(des.getOutputsOfOp()), "Outputs differ: %s vs. %s", orig.getOutputsOfOp(), des.getOutputsOfOp())

				Preconditions.checkState((orig.getControlDeps() Is Nothing) = (des.getControlDeps() Is Nothing), "Control dependencies differ: %s vs. %s", orig.getControlDeps(), des.getControlDeps())
'JAVA TO VB CONVERTER WARNING: LINQ 'SequenceEqual' is not always identical to Java AbstractList 'equals':
'ORIGINAL LINE: org.nd4j.common.base.Preconditions.checkState(orig.getControlDeps() == null || orig.getControlDeps().equals(des.getControlDeps()), "Control dependencies differ: %s vs. %s", orig.getControlDeps(), des.getControlDeps());
				Preconditions.checkState(orig.getControlDeps() Is Nothing OrElse orig.getControlDeps().SequenceEqual(des.getControlDeps()), "Control dependencies differ: %s vs. %s", orig.getControlDeps(), des.getControlDeps())

				Preconditions.checkState((orig.getVarControlDeps() Is Nothing) = (des.getVarControlDeps() Is Nothing), "Op variable control dependencies differ: %s vs. %s", orig.getVarControlDeps(), des.getVarControlDeps())
'JAVA TO VB CONVERTER WARNING: LINQ 'SequenceEqual' is not always identical to Java AbstractList 'equals':
'ORIGINAL LINE: org.nd4j.common.base.Preconditions.checkState(orig.getVarControlDeps() == null || orig.getVarControlDeps().equals(des.getVarControlDeps()), "Op variable control dependencies differ: %s vs. %s", orig.getControlDeps(), des.getControlDeps());
				Preconditions.checkState(orig.getVarControlDeps() Is Nothing OrElse orig.getVarControlDeps().SequenceEqual(des.getVarControlDeps()), "Op variable control dependencies differ: %s vs. %s", orig.getControlDeps(), des.getControlDeps())

				Preconditions.checkState((orig.getControlDepFor() Is Nothing) = (des.getControlDepFor() Is Nothing), "Op control dependencies for list differ: %s vs. %s", orig.getControlDepFor(), des.getControlDepFor())
'JAVA TO VB CONVERTER WARNING: LINQ 'SequenceEqual' is not always identical to Java AbstractList 'equals':
'ORIGINAL LINE: org.nd4j.common.base.Preconditions.checkState(orig.getControlDepFor() == null || orig.getControlDepFor().equals(des.getControlDepFor()), "Op variable control dependencies differ: %s vs. %s", orig.getControlDepFor(), des.getControlDepFor());
				Preconditions.checkState(orig.getControlDepFor() Is Nothing OrElse orig.getControlDepFor().SequenceEqual(des.getControlDepFor()), "Op variable control dependencies differ: %s vs. %s", orig.getControlDepFor(), des.getControlDepFor())

				Preconditions.checkState(orig.Op.GetType().Equals(des.Op.GetType()), "Classes differ: %s v. %s", orig.Op.GetType(), des.Op.GetType())
			Next s

			'Check placeholders:
			Dim phBefore As [Set](Of String) = New HashSet(Of String)()
			Dim phAfter As [Set](Of String) = New HashSet(Of String)()

			For Each v As Variable In original.getVariables().values()
				If v.getVariable().isPlaceHolder() Then
					phBefore.add(v.getName())
				End If
			Next v
			For Each v As Variable In deserialized.getVariables().values()
				If v.getVariable().isPlaceHolder() Then
					phAfter.add(v.getName())
				End If
			Next v

			If phBefore Is Nothing Then
				Preconditions.checkState(phAfter Is Nothing OrElse phAfter.size() = 0, "%s", phAfter)
			Else
				Preconditions.checkState(phAfter IsNot Nothing, "Placeholders after deserialization was null")
				Preconditions.checkState(phBefore.Equals(phAfter), "Before: %s, after deserialization: %s", phBefore, phAfter)
			End If

			Dim varsBefore As IDictionary(Of String, Variable) = original.getVariables()
			Dim varsAfter As IDictionary(Of String, Variable) = deserialized.getVariables()
			Preconditions.checkState(varsBefore.Keys.Equals(varsAfter.Keys), "Variable keysets do not match: %s vs %s", varsBefore.Keys, varsAfter.Keys)

	'        System.out.println(original.summary());
	'        System.out.println("\n\n\n\n");
	'        System.out.println(deserialized.summary());

			For Each s As String In varsBefore.Keys
				Dim vB As Variable = varsBefore(s)
				Dim vA As Variable = varsAfter(s)
				Preconditions.checkState(vB.getName().Equals(vA.getName()), "Variable names do not match: %s vs %s", vA.getName(), vB.getName())
				Preconditions.checkState(vB.getVariable().getVariableType() = vA.getVariable().getVariableType(), "Variable types do not match: %s - %s vs %s", s, vB.getVariable().getVariableType(), vA.getVariable().getVariableType())

				equalConsideringNull(vB.getInputsForOp(), vA.getInputsForOp(), "%s - Input to ops differ: %s vs. %s", s, vB.getInputsForOp(), vA.getInputsForOp())

				Preconditions.checkState((vB.getOutputOfOp() Is Nothing AndAlso vA.getOutputOfOp() Is Nothing) OrElse vB.getOutputOfOp().Equals(vA.getOutputOfOp()), "%s - Output of op differ: %s vs. %s", s, vB.getOutputOfOp(), vA.getOutputOfOp())

				equalConsideringNull(vB.getControlDeps(), vA.getControlDeps(), "%s - Control dependencies differ: %s vs. %s", s, vB.getControlDeps(), vA.getControlDeps())

				equalConsideringNull(vB.getControlDepsForOp(), vA.getControlDepsForOp(), "%s - Control dependencies for ops differ: %s vs. %s", s, vB.getControlDepsForOp(), vA.getControlDepsForOp())

				equalConsideringNull(vB.getControlDepsForVar(), vA.getControlDepsForVar(), "%s - Control dependencies for vars differ: %s vs. %s", s, vB.getControlDepsForVar(), vA.getControlDepsForVar())
			Next s

			'Check loss variables:
			Dim lossVarBefore As IList(Of String) = original.getLossVariables()
			Dim lossVarAfter As IList(Of String) = deserialized.getLossVariables()
			If lossVarBefore Is Nothing OrElse lossVarBefore.Count = 0 Then
				Preconditions.checkState(lossVarAfter Is Nothing OrElse lossVarAfter.Count = 0, "Loss variables ")
			Else
'JAVA TO VB CONVERTER WARNING: LINQ 'SequenceEqual' is not always identical to Java AbstractList 'equals':
'ORIGINAL LINE: org.nd4j.common.base.Preconditions.checkState(lossVarBefore.equals(lossVarAfter), "Loss variables are not equal after deserialization: %s vs %s", lossVarBefore, lossVarAfter);
				Preconditions.checkState(lossVarBefore.SequenceEqual(lossVarAfter), "Loss variables are not equal after deserialization: %s vs %s", lossVarBefore, lossVarAfter)
			End If

			If tc.fwdTestFns() IsNot Nothing AndAlso Not tc.fwdTestFns().isEmpty() Then
				'Finally: check execution/output
				Dim outOrig As IDictionary(Of String, INDArray) = original.outputAll(tc.placeholderValues())
				Dim outDe As IDictionary(Of String, INDArray) = deserialized.outputAll(tc.placeholderValues())
				Preconditions.checkState(outOrig.Keys.Equals(outDe.Keys), "Keysets for execution after deserialization does not match key set for original model")

				For Each s As String In outOrig.Keys
					Dim orig As INDArray = outOrig(s)
					Dim deser As INDArray = outDe(s)

					Dim f As [Function](Of INDArray, String) = tc.fwdTestFns().get(s)
					Dim err As String = Nothing
					If f IsNot Nothing Then
						err = f.apply(deser)
					Else
						If Not orig.Equals(deser) Then
							'Edge case: check for NaNs in original and deserialized... might be legitimate test (like replaceNaNs op)
							Dim count As Long = If(orig.dataType().isNumerical(), Nd4j.Executioner.execAndReturn(New MatchCondition(orig, Conditions.Nan)).getFinalResult().longValue(), -1)
							If orig.dataType().isNumerical() AndAlso count > 0 AndAlso orig.equalShapes(deser) Then
								Dim count2 As Long = Nd4j.Executioner.execAndReturn(New MatchCondition(deser, Conditions.Nan)).getFinalResult().longValue()
								If count <> count2 Then
									err = "INDArray equality failed"
								Else
									'TODO is there a better way to do this?
									Dim iter As New NdIndexIterator(orig.shape())
									Do While iter.MoveNext()
										Dim i() As Long = iter.Current
										Dim d1 As Double = orig.getDouble(i)
										Dim d2 As Double = deser.getDouble(i)
										If (Double.IsNaN(d1) <> Double.IsNaN(d2)) OrElse (Double.IsInfinity(d1) <> Double.IsInfinity(d2)) OrElse Math.Abs(d1 - d2) > 1e-5 Then
											err = "INDArray equality failed"
											Exit Do
										End If
									Loop
								End If
							Else
								err = "INDArray equality failed"
							End If
						End If
					End If

					Preconditions.checkState(err Is Nothing, "Variable result (%s) failed check - ""%ndSInfo"" vs ""%ndSInfo"" - %nd10 vs %nd10" & vbLf & "Error:%s", s, orig, deser, orig, deser, err)
				Next s
			End If
		End Sub

		Protected Friend Shared Sub equalConsideringNull(ByVal l1 As IList(Of String), ByVal l2 As IList(Of String), ByVal msg As String, ParamArray ByVal args() As Object)
			'Consider null and length 0 list to be equal (semantically they mean the same thing)
			Dim empty1 As Boolean = l1 Is Nothing OrElse l1.Count = 0
			Dim empty2 As Boolean = l2 Is Nothing OrElse l2.Count = 0
			If empty1 AndAlso empty2 Then
				Return
			End If
'JAVA TO VB CONVERTER WARNING: LINQ 'SequenceEqual' is not always identical to Java AbstractList 'equals':
'ORIGINAL LINE: org.nd4j.common.base.Preconditions.checkState(l1 == null || l1.equals(l2), msg, args);
			Preconditions.checkState(l1 Is Nothing OrElse l1.SequenceEqual(l2), msg, args)
		End Sub

		''' <summary>
		''' Validate the outputs of a single op
		''' </summary>
		''' <param name="testCase"> Op test case to run </param>
		''' <returns> NULL if test is OK, or an error message otherwise </returns>
'JAVA TO VB CONVERTER NOTE: The parameter testCase was renamed since it may cause conflicts with calls to static members of the user-defined type with this name:
		Public Shared Function validate(ByVal testCase_Conflict As OpTestCase) As String
			collectCoverageInformation(testCase_Conflict)

			'Check shape function:
			Dim outShapes As IList(Of LongShapeDescriptor)
			Try
				outShapes = Nd4j.Executioner.calculateOutputShape(testCase_Conflict.op())
			Catch t As Exception
				Throw New System.InvalidOperationException("Error calculating output shapes during op validation", t)
			End Try

			If outShapes.Count <> testCase_Conflict.testFns().size() Then
				Return "Expected number of output shapes and number of outputs differ. " & outShapes.Count & " output shapes," & " but OpTestCase specifies " & testCase_Conflict.testFns().size() & " outputs expected"
			End If

			For i As Integer = 0 To outShapes.Count - 1
				Dim act As val = outShapes(i)
				Dim exp As val = testCase_Conflict.expShapes().get(i)
				If Not Objects.equals(exp.dataType(), act.dataType()) Then
					Return "Shape function check failed for output " & i & ": expected shape " & exp & ", actual shape " & act
				End If
				If Not act.getShape().SequenceEqual(exp.getShape()) Then
					Return "Shape function check failed for output " & i & ": expected shape " & exp & ", actual shape " & act
				End If
			Next i

			'Check the outputs:
			Try
				Nd4j.Executioner.execAndReturn(testCase_Conflict.op())
			Catch t As Exception
				Throw New System.InvalidOperationException("Error during op execution", t)
			End Try

			Dim i As Integer = 0
			Do While i < testCase_Conflict.testFns().size()
				Dim [error] As String
				Try
					[error] = testCase_Conflict.testFns().get(i).apply(testCase_Conflict.op().outputArguments().get(i))
				Catch t As Exception
					Throw New System.InvalidOperationException("Exception thrown during op output validation for output " & i, t)
				End Try

				If [error] IsNot Nothing Then
					Return "Output " & i & " failed: " & [error]
				End If
				i += 1
			Loop

			Return Nothing 'OK
		End Function


		'==================================================================================================================
		' Coverage information

		Private Shared allOps As IList(Of Type)
		Private Shared nonMappedLibnd4jOps As IList(Of Long)
		Private Shared dedupedCustomOps As IDictionary(Of Long, Pair(Of IList(Of String), CustomOpDescriptor))
		Private Shared countTotalLibnd4jOps As Integer
		Private Shared gradCheckCoverageCountPerClass As IDictionary(Of Type, Integer) = New LinkedHashMap(Of Type, Integer)()
		Private Shared fwdPassCoverageCountPerClass As IDictionary(Of Type, Integer) = New LinkedHashMap(Of Type, Integer)()
		Private Shared singleOpTestCountPerClass As IDictionary(Of Type, Integer) = New LinkedHashMap(Of Type, Integer)()
		Private Shared opsWithTFMappingTFImportCounts As IDictionary(Of Type, Integer) = New LinkedHashMap(Of Type, Integer)()
		Private Shared tfMappedOpsImportTestCounts As IDictionary(Of String, Integer) = New LinkedHashMap(Of String, Integer)()


'JAVA TO VB CONVERTER NOTE: The parameter testCase was renamed since it may cause conflicts with calls to static members of the user-defined type with this name:
		Private Shared Sub collectCoverageInformation(ByVal testCase_Conflict As TestCase)
			Dim sd As SameDiff = testCase_Conflict.sameDiff()

			'NOTE: Count on a per-test-case basis, not on a 'per function seen' basis
			'i.e., don't double count if a SameDiff instance has multiple copies of the same op type

			'Collect coverage information for backprop:
			Dim functions() As DifferentialFunction = sd.ops()
			Dim backpropSeen As [Set](Of Type) = New HashSet(Of Type)()
			For Each df As DifferentialFunction In functions
				backpropSeen.add(df.GetType())
			Next df
			For Each c As Type In backpropSeen
				If gradCheckCoverageCountPerClass.ContainsKey(c) Then
					gradCheckCoverageCountPerClass(c) = gradCheckCoverageCountPerClass(c) + 1
				Else
					gradCheckCoverageCountPerClass(c) = 1
				End If
			Next c

			'Collect coverage information for forward pass (expected outputs)
			Dim seen As [Set](Of Type) = Nothing
			If testCase_Conflict.fwdTestFns() IsNot Nothing Then
				For Each s As String In testCase_Conflict.fwdTestFns().keySet()
					'Determine the differential function that this variable is the output of, if any
					Dim df As DifferentialFunction = sd.getVariableOutputOp(s)
					If df IsNot Nothing Then
						If seen Is Nothing Then
							seen = New HashSet(Of Type)()
						End If

						seen.add(df.GetType())
					End If
				Next s
			End If

			If seen IsNot Nothing Then
				For Each c As Type In seen
					If fwdPassCoverageCountPerClass.ContainsKey(c) Then
						fwdPassCoverageCountPerClass(c) = fwdPassCoverageCountPerClass(c) + 1
					Else
						fwdPassCoverageCountPerClass(c) = 1
					End If
				Next c
			End If
		End Sub

'JAVA TO VB CONVERTER NOTE: The parameter testCase was renamed since it may cause conflicts with calls to static members of the user-defined type with this name:
		Private Shared Sub collectCoverageInformation(ByVal testCase_Conflict As OpTestCase)
			'TODO we're basically assuming subtypes of DynamicCustomOp here, for coverage... not DCO itself
			If singleOpTestCountPerClass.ContainsKey(testCase_Conflict.op().GetType()) Then
				singleOpTestCountPerClass(testCase_Conflict.op().GetType()) = singleOpTestCountPerClass(testCase_Conflict.op().GetType()) + 1
			Else
				singleOpTestCountPerClass(testCase_Conflict.op().GetType()) = 1
			End If
		End Sub


		Public Shared Sub collectTensorflowImportCoverage(ByVal graph As SameDiff)
			For Each op As SameDiffOp In graph.getOps().values()
				Dim d As DifferentialFunction = op.Op
				Dim tfNames() As String = Nothing
				Try
					tfNames = d.tensorflowNames()
				Catch t As Exception
					'Ignore
					Continue For
				End Try

				If tfNames IsNot Nothing AndAlso tfNames.Length > 0 Then
					Dim currCount As Integer? = opsWithTFMappingTFImportCounts(d.GetType())
					If currCount Is Nothing Then
						currCount = 0
					End If
					currCount += 1
					opsWithTFMappingTFImportCounts(d.GetType()) = currCount

					currCount = fwdPassCoverageCountPerClass(d.GetType())
					If currCount Is Nothing Then
						currCount = 0
					End If
					currCount += 1
					fwdPassCoverageCountPerClass(d.GetType()) = currCount

					For Each s As String In tfNames
						currCount = tfMappedOpsImportTestCounts(s)
						If currCount Is Nothing Then
							currCount = 0
						End If
						currCount += 1
						tfMappedOpsImportTestCounts(s) = currCount
					Next s
				End If
			Next op

		End Sub

		'Collect coverage information
		Shared Sub New()
			initializeCoverage()
		End Sub

		Private Shared Sub initializeCoverage()
			'Scan classpath to find all DifferentialFunction instances, so tensorflow/onnx mappings can be made
			'We're assuming here that all instances with such mappings are defined in ND4J
			'As of 04/2018 all DifferentialFunction classes are defined in org.nd4j.linalg.api.ops - with the exception
			' of ILossFunction instances, which don't have TF/Onnx import working anyway
			Dim info As ImmutableSet(Of ClassPath.ClassInfo)
			Try
				'Dependency note: this ClassPath class was added in Guava 14
				info = ClassPath.from(GetType(DifferentialFunctionClassHolder).getClassLoader()).getTopLevelClassesRecursive("org.nd4j.linalg.api.ops")
			Catch e As IOException
				'Should never happen
				Throw New Exception(e)
			End Try

			'Also, info for libnd4j op mapping:
			Dim customOps As IDictionary(Of String, CustomOpDescriptor) = Nd4j.Executioner.getCustomOperations()

			'De-duplicate custom ops based on hash (due to aliases also being returned)
			dedupedCustomOps = New Dictionary(Of Long, Pair(Of IList(Of String), CustomOpDescriptor))()
			For Each e As KeyValuePair(Of String, CustomOpDescriptor) In customOps.SetOfKeyValuePairs()
				Dim hash As Long = e.Value.getHash()
				If Not dedupedCustomOps.ContainsKey(hash) Then
					Dim p As New Pair(Of IList(Of String), CustomOpDescriptor)(New List(Of String)(), e.Value)
					dedupedCustomOps(hash) = p
				End If
				Dim p As Pair(Of IList(Of String), CustomOpDescriptor) = dedupedCustomOps(hash)
				Dim l As IList(Of String) = p.First
				If Not l.Contains(e.Key) Then
					l.Add(e.Key)
				End If
			Next e

			Dim notSeenCustomOps As [Set](Of Long) = New HashSet(Of Long)(dedupedCustomOps.Keys)

			allOps = New List(Of Type)(gradCheckCoverageCountPerClass.Keys)
			For Each c As ClassPath.ClassInfo In info
				'Load method: Loads (but doesn't link or initialize) the class.
				Dim clazz As Type = ND4JClassLoading.loadClassByName(c.getName())
				Objects.requireNonNull(clazz)

				If Modifier.isAbstract(clazz.getModifiers()) OrElse clazz.IsInterface OrElse Not clazz.IsAssignableFrom(GetType(DifferentialFunction)) Then
					Continue For
				End If

				If clazz.IsAssignableFrom(GetType(DifferentialFunction)) AndAlso Not clazz.Name.Contains("Old") Then 'Exclude OldSubOp, etc
					allOps.Add(clazz)
				End If

				Dim opName As String = Nothing
				Try
					opName = CType(System.Activator.CreateInstance(clazz), DifferentialFunction).opName()
				Catch e As Exception
'JAVA TO VB CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
					log.warn("Could not instantiate object of type {}", clazz.FullName, e)
				End Try

				If opName IsNot Nothing Then
					Dim d As CustomOpDescriptor = customOps(opName)
					If d IsNot Nothing Then
						notSeenCustomOps.remove(d.getHash())
					End If
				End If
			Next c

			countTotalLibnd4jOps = dedupedCustomOps.Count
			nonMappedLibnd4jOps = New List(Of Long)(notSeenCustomOps)
			nonMappedLibnd4jOps.Sort(New ComparatorAnonymousInnerClass())

			allOps.Sort(New ComparatorAnonymousInnerClass2())
			For Each c As Type In allOps
				gradCheckCoverageCountPerClass(c) = 0
				fwdPassCoverageCountPerClass(c) = 0
				singleOpTestCountPerClass(c) = 0
			Next c
		End Sub

		Private Class ComparatorAnonymousInnerClass
			Implements IComparer(Of Long)

			Public Function Compare(ByVal o1 As Long?, ByVal o2 As Long?) As Integer Implements IComparer(Of Long).Compare
				Dim p1 As Pair(Of IList(Of String), CustomOpDescriptor) = dedupedCustomOps(o1)
				Dim p2 As Pair(Of IList(Of String), CustomOpDescriptor) = dedupedCustomOps(o2)
				Return p1.getKey().get(0).compareTo(p2.getKey().get(0))
			End Function
		End Class

		Private Class ComparatorAnonymousInnerClass2
			Implements IComparer(Of Type)

			Public Function Compare(ByVal o1 As Type, ByVal o2 As Type) As Integer Implements IComparer(Of [Class]).Compare
'JAVA TO VB CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
				Return String.CompareOrdinal(o1.FullName, o2.FullName)
			End Function
		End Class

		''' <summary>
		''' Log the coverage information
		''' </summary>
		''' <param name="logAdequatelyTested"> If true: log details of each op that has both forward and (if appropriate) backward tests </param>
		''' <param name="logInadequate">       If false: log details of each op that does NOT have both forward and (if appropriate) backward tests </param>
		Public Shared Sub logCoverageInformation(ByVal logAdequatelyTested As Boolean, ByVal logInadequate As Boolean, ByVal logUnmappedLibnd4jOps As Boolean, ByVal logUntestedTFImport As Boolean, ByVal logUnmappedTFOps As Boolean)
			'Set of ops that we can't gradient check
			Dim excludedFromBackpropCoverage As [Set](Of Type) = excludedFromGradientCheckCoverage()
			Dim excludedFromAllTestCoverage As [Set](Of Type) = excludedFromAllTests()

			Dim numFormat As String = "%3d"
			Dim countAdequate As Integer = 0
			Dim countAdequateBwd As Integer = 0
			Dim countAdequateFwd As Integer = 0
			If logAdequatelyTested Then
				log.info(" --- Adequately Tested Classes ---")
				For Each c As Type In allOps
					If excludedFromAllTestCoverage.contains(c) Then
						Continue For
					End If

					Dim countBackpropSeen As Integer = gradCheckCoverageCountPerClass(c)
					Dim countFwdValidation As Integer = fwdPassCoverageCountPerClass(c) + singleOpTestCountPerClass(c)

					If countBackpropSeen > 0 Then
						countAdequateBwd += 1
					End If
					If countFwdValidation > 0 Then
						countAdequateFwd += 1
					End If
					If countFwdValidation > 0 AndAlso countBackpropSeen > 0 Then
						countAdequate += 1
					End If

					Dim gradExcluded As Boolean = excludedFromBackpropCoverage.contains(c)
					If countFwdValidation > 0 AndAlso (countBackpropSeen > 0 OrElse gradExcluded) Then
						'At least 1 forward test, and 1 gradient check

						If gradExcluded Then
'JAVA TO VB CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
							log.info("Forward: {} tests, GradCheck: <excluded> for op {}", String.format(numFormat, countFwdValidation), c.FullName)
						Else
'JAVA TO VB CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
							log.info("Forward: {} tests, GradCheck: {} tests  for op {}", String.format(numFormat, countFwdValidation), String.format(numFormat, countBackpropSeen), c.FullName)
						End If
					End If
				Next c
			End If

			If logInadequate Then
				log.info(" --- Classes NOT Tested Adequately ---")
				For Each c As Type In allOps
					If excludedFromAllTestCoverage.contains(c) Then
						Continue For
					End If
					Dim countBackpropSeen As Integer = gradCheckCoverageCountPerClass(c)
					Dim countFwdValidation As Integer = fwdPassCoverageCountPerClass(c) + singleOpTestCountPerClass(c)

					Dim gradExcluded As Boolean = excludedFromBackpropCoverage.contains(c)
					If countFwdValidation = 0 OrElse (countBackpropSeen = 0 AndAlso Not gradExcluded) Then
						'0 forward test OR 0 gradient check (and not excluded from grad checks)

						If gradExcluded Then
'JAVA TO VB CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
							log.info("Forward: {} tests, GradCheck: <excluded> for op {}", String.format(numFormat, countFwdValidation), c.FullName)
						Else
'JAVA TO VB CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
							log.info("Forward: {} tests, GradCheck: {} tests  for op {}", String.format(numFormat, countFwdValidation), String.format(numFormat, countBackpropSeen), c.FullName)
						End If
					End If
				Next c
			End If

			Dim countLibnd4jIgnored As Integer = 0
			If logUnmappedLibnd4jOps Then
				Dim ignoreLibnd4j As [Set](Of String) = excludeFromLibnd4jCustomOpMapping()
				log.info(" --- Libnd4j Ops Not Mapped ---")
				For Each l As Long In nonMappedLibnd4jOps
					Dim p As Pair(Of IList(Of String), CustomOpDescriptor) = dedupedCustomOps(l)
					Dim foundIgnore As Boolean = False
					For Each s As String In p.First
						If ignoreLibnd4j.contains(s) Then
							foundIgnore = True
							countLibnd4jIgnored += 1
							Exit For
						End If
					Next s
					If foundIgnore Then
						Continue For
					End If
					log.info("Not mapped libnd4j custom op: {} (hash: {})", p.First, l)
				Next l
			End If

			'Log info for TF import op coverage:
			Dim tfOpsMap As IDictionary(Of String, DifferentialFunction) = DifferentialFunctionClassHolder.Instance.getTensorFlowNames()
			Dim totalTFMappedOps As Integer = tfOpsMap.Count
			Dim tfOpsWithImportTests As Integer = 0
			If logUntestedTFImport Then
				log.info(" --- Ops with TF Mapping but No TF Import Tests ---")
			End If
			Dim tfOpsKeys As IList(Of String) = New List(Of String)(tfOpsMap.Keys)
			tfOpsKeys.Sort()
			Dim tfIgnored As [Set](Of String) = excludeFromTfImportCoverage()
			Dim tfImportIgnored As Integer = 0
			For Each s As String In tfOpsKeys
				Dim count As Integer? = tfMappedOpsImportTestCounts(s)
				If count Is Nothing OrElse count = 0 Then
					If tfIgnored.contains(s) Then
						tfImportIgnored += 1
					ElseIf logUntestedTFImport Then
						log.info("TF mapped op with no import tests: {}", s)
					End If
				Else
					tfOpsWithImportTests += 1
				End If
			Next s

			If logUnmappedTFOps Then
				log.info(" --- TF Ops Not Mapped for Import ---")
				Dim allTFOps As IDictionary(Of String, OpDef)
				Try
					allTFOps = TensorflowDescriptorParser.opDescs()
				Catch t As Exception
					Throw New Exception(t)
				End Try

				Dim notMapped As IList(Of String) = New List(Of String)()
				For Each s As String In allTFOps.Keys
					If DifferentialFunctionClassHolder.Instance.getOpWithTensorflowName(s) Is Nothing AndAlso Not tfIgnored.contains(s) Then
						notMapped.Add(s)
					End If
				Next s

				notMapped.Sort()
				Dim subsets As Integer = CInt(Math.Ceiling(notMapped.Count \ 10))
				For i As Integer = 0 To subsets - 1
					log.info("TF ops not mapped for import: {}", notMapped.subList(10*i, Math.Min(10*(i+1), notMapped.Count)))
				Next i
			End If


			Dim totalFwd As Integer = 0
			For Each c As Type In allOps
				If Not excludedFromAllTestCoverage.contains(c) Then
					totalFwd += 1
				End If
			Next c
			Dim totalBwd As Integer = 0
			For Each c As Type In allOps
				If Not isBackpropOp(c) Then
					totalBwd += 1
				End If
			Next c

			Dim fracFwdAdequate As Double = countAdequateFwd / CDbl(totalFwd)
			Dim fracBwdAdequate As Double = countAdequateBwd / CDbl(totalBwd)
			Dim fracAdequate As Double = countAdequate / CDbl(allOps.Count)
			Dim pcFwd As String = String.Format("{0:F2}", fracFwdAdequate * 100.0)
			Dim pcBwd As String = String.Format("{0:F2}", fracBwdAdequate * 100.0)
			Dim pc As String = String.Format("{0:F2}", fracAdequate * 100.0)

			Dim countTf As Integer = DifferentialFunctionClassHolder.Instance.getCountTotalTfOps()
			Dim countTfMapped As Integer = DifferentialFunctionClassHolder.Instance.getCountTotalMappedOps()
			Dim tfFrac As Double = countTfMapped / CDbl(countTf)
			Dim fracTfStr As String = String.Format("{0:F2}", 100.0 * tfFrac)

			Dim countLibnd4jMapped As Integer = countTotalLibnd4jOps - nonMappedLibnd4jOps.Count
			Dim fracLibnd4j As String = String.Format("{0:F2}", 100.0 * (countLibnd4jMapped / CDbl(countTotalLibnd4jOps - countLibnd4jIgnored)))

			Dim fracTFMappedTested As String = String.Format("{0:F2}", 100.0 * tfOpsWithImportTests / CDbl(totalTFMappedOps-tfImportIgnored))

			log.info("*****************************************************")
			log.info("Op Validation:                        {} of {} classes with adequate tests ({}% coverage)", countAdequate, totalFwd, pc)
			log.info("Forward pass tests:                   {} of {} classes ({}% coverage)", countAdequateFwd, totalFwd, pcFwd)
			log.info("Gradient check tests:                 {} of {} classes ({}% coverage)", countAdequateBwd, totalBwd, pcBwd)
			log.info("({} ops excluded from gradient check coverage)", excludedFromBackpropCoverage.size())
			log.info("({} ops excluded from fwd+gradient tests)", excludedFromAllTestCoverage.size())
			log.info("TF mapped ops:                        {} of {} ({}%)", countTfMapped, countTf, fracTfStr)
			log.info("SD ops with TF import mapping + test  {} of {} ({}%) - {} ignored for coverage", tfOpsWithImportTests, (totalTFMappedOps-tfImportIgnored), fracTFMappedTested, tfImportIgnored)
			log.info("Libnd4j mapped ops:                   {} of {} ({}%) - {} excluded for coverage", countLibnd4jMapped, countTotalLibnd4jOps, fracLibnd4j, countLibnd4jIgnored)
			log.info("*****************************************************")
		End Sub

		Private Shared Function isBackpropOp(ByVal c As Type) As Boolean
			Dim name As String = c.Name
			Return name.Contains("Bp") OrElse name.Contains("Derivative") OrElse name.Contains("Grad")
		End Function


		Private Shared Function excludedFromAllTests() As [Set](Of [Class])
			Dim list As System.Collections.IList = java.util.Arrays.asList(GetType(DynamicCustomOp), GetType(GradientBackwardsMarker), GetType(EqualsWithEps), GetType(FreeGridOp), GetType(MergeSum), GetType(ScalarRemainder), GetType(RestoreV2), GetType(SaveV2), GetType(ScalarSetValue), GetType(BinomialDistributionEx), GetType(BroadcastAMax), GetType(BroadcastAMin), GetType(BroadcastAddOp), GetType(BroadcastCopyOp), GetType(BroadcastDivOp), GetType(BroadcastEqualTo), GetType(BroadcastGreaterThan), GetType(BroadcastGreaterThanOrEqual), GetType(BroadcastLessThan), GetType(BroadcastLessThanOrEqual), GetType(BroadcastMax), GetType(BroadcastMin), GetType(BroadcastMulOp), GetType(BroadcastNotEqual), GetType(BroadcastRDivOp), GetType(BroadcastRSubOp), GetType(BroadcastSubOp), GetType(AddBpOp), GetType(DivBpOp), GetType(FloorDivBpOp), GetType(FloorModBpOp), GetType(MulBpOp), GetType(RDivBpOp), GetType(RSubBpOp), GetType(SquaredDifferenceBpOp), GetType(SubBpOp), GetType(CumProdBp), GetType(DotBp), GetType(SquaredNormBp), GetType(SoftmaxBp), GetType(CubeDerivative), GetType(GELUDerivative), GetType(PreciseGELUDerivative), GetType(HardSigmoidDerivative), GetType(HardTanhDerivative), GetType(LeakyReLUDerivative), GetType(LogSoftMaxDerivative), GetType(RationalTanhDerivative), GetType(RectifiedTanhDerivative), GetType(Relu6Derivative), GetType(PReluBp), GetType(SELUDerivative), GetType(SigmoidDerivative), GetType(org.nd4j.linalg.api.ops.impl.transforms.strict.SigmoidDerivative), GetType(SoftSignDerivative), GetType(TanhDerivative), GetType(SwishDerivative), GetType(TanDerivative), GetType(TanhDerivative), GetType(org.nd4j.linalg.api.ops.impl.transforms.strict.TanhDerivative), GetType(PowDerivative), GetType(org.nd4j.linalg.api.ops.impl.scalar.RectifiedLinearDerivative), GetType(org.nd4j.linalg.api.ops.impl.transforms.gradient.CubeBp), GetType(org.nd4j.linalg.api.ops.impl.transforms.gradient.EluBp), GetType(org.nd4j.linalg.api.ops.impl.transforms.gradient.HardSigmoidBp), GetType(org.nd4j.linalg.api.ops.impl.transforms.gradient.HardTanhBp), GetType(org.nd4j.linalg.api.ops.impl.transforms.gradient.LeakyReLUBp), GetType(org.nd4j.linalg.api.ops.impl.transforms.gradient.RationalTanhBp), GetType(org.nd4j.linalg.api.ops.impl.transforms.gradient.RectifiedTanhBp), GetType(org.nd4j.linalg.api.ops.impl.transforms.gradient.SeluBp), GetType(org.nd4j.linalg.api.ops.impl.transforms.gradient.SoftPlusBp), GetType(org.nd4j.linalg.api.ops.impl.transforms.gradient.SoftSignBp), GetType(org.nd4j.linalg.api.ops.impl.transforms.gradient.ThresholdReluBp), GetType(org.nd4j.linalg.api.ops.impl.transforms.pairwise.arithmetic.bp.ModBpOp), GetType(BiasAddGrad), GetType(ConcatBp), GetType(TileBp), GetType(BatchNormDerivative), GetType(Conv2DDerivative), GetType(Conv3DDerivative), GetType(DeConv2DDerivative), GetType(LocalResponseNormalizationDerivative), GetType(Pooling2DDerivative), GetType(Pooling3DDerivative), GetType(SConv2DDerivative), GetType(Upsampling2dDerivative), GetType(Im2colBp), GetType(SliceBp), GetType(StridedSliceBp), GetType(MmulBp), GetType(DotProductAttentionBp), GetType(MultiHeadDotProductAttentionBp), GetType(LayerNormBp), GetType(StandardizeBp), GetType(DynamicPartitionBp), GetType(AbsoluteDifferenceLossBp), GetType(CosineDistanceLossBp), GetType(HingeLossBp), GetType(HuberLossBp), GetType(LogLossBp), GetType(LogPoissonLossBp), GetType(MeanPairwiseSquaredErrorLossBp), GetType(MeanSquaredErrorLossBp), GetType(SigmoidCrossEntropyLossBp), GetType(SoftmaxCrossEntropyLossBp), GetType(SparseSoftmaxCrossEntropyLossWithLogitsBp), GetType(SegmentMaxBp), GetType(SegmentMeanBp), GetType(SegmentMinBp), GetType(SegmentProdBp), GetType(SegmentSumBp), GetType(UnsortedSegmentMaxBp), GetType(UnsortedSegmentMeanBp), GetType(UnsortedSegmentMinBp), GetType(UnsortedSegmentProdBp), GetType(UnsortedSegmentSqrtNBp), GetType(UnsortedSegmentSumBp), GetType(ExternalErrorsFunction), GetType(InvertedPredicateMetaOp), GetType(PostulateMetaOp), GetType(PredicateMetaOp), GetType(ReduceMetaOp), GetType(BarnesEdgeForces), GetType(BarnesHutGains), GetType(BarnesHutSymmetrize), GetType(SpTreeCell), GetType(CbowRound), GetType(SkipGramRound), GetType(HashCode), GetType(HashCode), GetType(BitCast), GetType(ToggleBits))

			Return New HashSet(Of Type)(list)
		End Function

		''' <summary>
		''' Returns a list of classes that are not gradient checkable.
		''' An operation may not be gradient checkable due to, for example:
		''' (a) Having no real-valued arguments<br>
		''' (b) Having random output (dropout, for example)<br>
		''' <para>
		''' Note that hawving non-real-valued output is OK - we still want to test these, as they
		''' should pass back zero gradients!
		''' </para>
		''' </summary>
		Private Shared Function excludedFromGradientCheckCoverage() As [Set](Of [Class])
			Dim list As System.Collections.IList = java.util.Arrays.asList(GetType(DynamicCustomOp), GetType(EqualsWithEps), GetType(ConfusionMatrix), GetType(Eye), GetType(OneHot), GetType(BinaryMinimalRelativeError), GetType(BinaryMinimalRelativeError), GetType(InvertPermutation), GetType(ConfusionMatrix), GetType(Linspace), GetType(Assert), GetType(Any), GetType(All), GetType(IsInf), GetType(org.nd4j.linalg.api.ops.impl.transforms.bool.IsInf), GetType(IsNaN), GetType(org.nd4j.linalg.api.ops.impl.transforms.bool.IsNaN), GetType(BooleanNot), GetType([Not]), GetType(MatchConditionTransform), GetType(InTopK), GetType(IsNonDecreasing), GetType(IsStrictlyIncreasing), GetType(IsNumericTensor), GetType(FirstIndex), GetType(LastIndex), GetType(ArgMax), GetType(ArgMin), GetType(Shape), GetType(ShapeN), GetType(SizeAt), GetType(BroadcastDynamicShape), GetType(ReductionShape), GetType(ShiftBits), GetType(RShiftBits), GetType(BitsHammingDistance), GetType(CyclicShiftBits), GetType(CyclicRShiftBits), GetType(RandomStandardNormal), GetType(DistributionUniform), GetType(AlphaDropOut), GetType(BernoulliDistribution), GetType(BinomialDistribution), GetType(BinomialDistributionEx), GetType(Choice), GetType(DropOut), GetType(DropOutInverted), GetType(GaussianDistribution), GetType(LogNormalDistribution), GetType(ProbablisticMerge), GetType(Range), GetType(TruncatedNormalDistribution), GetType(UniformDistribution), GetType(Col2Im), GetType(NormalizeMoments), GetType(CumProdBp), GetType(CumSumBp), GetType(DotBp), GetType(MaxBp), GetType(MeanBp), GetType(MinBp), GetType(Norm1Bp), GetType(Norm2Bp), GetType(NormMaxBp), GetType(ProdBp), GetType(StandardDeviationBp), GetType(SumBp), GetType(VarianceBp), GetType(LogicalAnd), GetType(LogicalNot), GetType(LogicalOr), GetType(LogicalXor), GetType(Histogram))

			Return New HashSet(Of Type)(list)
		End Function

		''' <summary>
		''' These ops are excluded from TF import test coverage, for various reasons
		''' </summary>
		Private Shared Function excludeFromTfImportCoverage() As [Set](Of String)
			Dim list As IList(Of String) = New List(Of String) From {"Reverse", "LogSigmoid", "HardSigmoid", "SpaceToBatch", "BatchToSpace", "Pad", "TopK", "InTopK", "BatchMatrixDeterminant", "BatchMatrixDiagPart", "BatchMatrixDiag", "BatchMatrixBandPart", "BatchMatrixInverse", "BatchMatrixSetDiag", "BatchMatrixSolve", "BatchMatrixSolveLs", "BatchMatrixTriangularSolve", "BatchSelfAdjointEig", "BatchSelfAdjointEigV2", "BatchSvd", "ExperimentalBytesProducedStatsDataset", "ExperimentalCSVDataset", "ExperimentalDatasetCardinality", "ExperimentalDatasetToTFRecord", "ExperimentalDenseToSparseBatchDataset", "ExperimentalDirectedInterleaveDataset", "ExperimentalGroupByReducerDataset", "ExperimentalGroupByWindowDataset", "ExperimentalIdentityIndexedDataset", "ExperimentalIgnoreErrorsDataset", "ExperimentalIndexedDatasetGet", "ExperimentalIndexedDatasetMaterialize", "ExperimentalIteratorGetDevice", "ExperimentalLMDBDataset", "ExperimentalLatencyStatsDataset", "ExperimentalMapAndBatchDataset", "ExperimentalMapDataset", "ExperimentalMatchingFilesDataset", "ExperimentalMaterializedIndexDatasetHandle", "ExperimentalMaxIntraOpParallelismDataset", "ExperimentalNonSerializableDataset", "ExperimentalNumaMapAndBatchDataset", "ExperimentalParallelInterleaveDataset", "ExperimentalParseExampleDataset", "ExperimentalPrivateThreadPoolDataset", "ExperimentalRandomDataset", "ExperimentalScanDataset", "ExperimentalSetStatsAggregatorDataset", "ExperimentalSleepDataset", "ExperimentalSlidingWindowDataset", "ExperimentalSqlDataset", "ExperimentalStatsAggregatorHandle", "ExperimentalStatsAggregatorSummary", "ExperimentalThreadPoolDataset", "ExperimentalThreadPoolHandle", "ExperimentalUnbatchDataset", "ExperimentalUniqueDataset", "DebugIdentity", "NcclAllReduce", "NcclBroadcast", "NcclReduce", "PyFunc", "PyFuncStateless", "QuantizedAdd", "QuantizedAvgPool", "QuantizedBatchNormWithGlobalNormalization", "QuantizedBiasAdd", "QuantizedConcat", "QuantizedConv2D", "QuantizedInstanceNorm", "QuantizedMatMul", "QuantizedMaxPool", "QuantizedMul", "QuantizedRelu", "QuantizedRelu6", "QuantizedReluX", "QuantizedReshape", "QuantizedResizeBilinear", "HardTanh", "Swish", "RDiv", "DivScalar", "LogX", "RationalTanh", "absargmax", "absargmin", "entropy_shannon", "count_zero", "SaveV2", "LoadV2", "RestoreV2", "RandomCrop"}

			Return New HashSet(Of String)(list)
		End Function


		''' <summary>
		''' These ops are ones we will never map at Java level for one reason or another
		''' </summary>
		Private Shared Function excludeFromLibnd4jCustomOpMapping() As [Set](Of String)
			Dim [out] As [Set](Of String) = New HashSet(Of String)()
			Collections.addAll([out], "TestOp2i2o", "testop2i2o", "firas_sparse", "test_output_reshape", "test_scalar", "testcustom", "testreduction", "to_double", "to_float16", "to_float32", "to_int32", "to_int64", "to_uint32", "to_uint64")

			Return [out]
		End Function

	End Class

End Namespace