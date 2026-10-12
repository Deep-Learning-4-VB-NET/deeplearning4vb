Imports System
Imports System.Collections.Generic
Imports DoubleArrayList = it.unimi.dsi.fastutil.doubles.DoubleArrayList
Imports lombok
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports DL4JClassLoading = org.deeplearning4j.common.config.DL4JClassLoading
Imports Layer = org.deeplearning4j.nn.api.Layer
Imports ConvolutionLayer = org.deeplearning4j.nn.conf.layers.ConvolutionLayer
Imports SubsamplingLayer = org.deeplearning4j.nn.conf.layers.SubsamplingLayer
Imports MultiLayerNetwork = org.deeplearning4j.nn.multilayer.MultiLayerNetwork
Imports CollectScoresListener = org.deeplearning4j.optimize.listeners.CollectScoresListener
Imports Preconditions = org.nd4j.common.base.Preconditions
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports MatchCondition = org.nd4j.linalg.api.ops.impl.reduce.longer.MatchCondition
Imports DataSet = org.nd4j.linalg.dataset.DataSet
Imports DataSetIterator = org.nd4j.linalg.dataset.api.iterator.DataSetIterator
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports BooleanIndexing = org.nd4j.linalg.indexing.BooleanIndexing
Imports Conditions = org.nd4j.linalg.indexing.conditions.Conditions
Imports Transforms = org.nd4j.linalg.ops.transforms.Transforms
Imports org.junit.jupiter.api.Assertions

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

Namespace org.deeplearning4j


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j public class LayerHelperValidationUtil
	Public Class LayerHelperValidationUtil

		Public Const MAX_REL_ERROR As Double = 1e-5
		Public Const MIN_ABS_ERROR As Double = 1e-6

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @AllArgsConstructor @NoArgsConstructor @Data @Builder public static class TestCase
		Public Class TestCase
			Friend testName As String
			Friend allowHelpersForClasses As IList(Of Type)
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Builder.@Default private boolean testForward = true;
			Friend testForward As Boolean = True
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Builder.@Default private boolean testScore = true;
			Friend testScore As Boolean = True
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Builder.@Default private boolean testBackward = true;
			Friend testBackward As Boolean = True
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Builder.@Default private boolean testTraining = false;
			Friend testTraining As Boolean = False
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Builder.@Default private double minAbsError = MIN_ABS_ERROR;
			Friend minAbsError As Double = MIN_ABS_ERROR
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Builder.@Default private double maxRelError = MAX_REL_ERROR;
			Friend maxRelError As Double = MAX_REL_ERROR
			Friend features As INDArray
			Friend labels As INDArray
			Friend data As DataSetIterator
		End Class

		Public Shared Sub disableCppHelpers()
			Try
				Dim clazz As Type = DL4JClassLoading.loadClassByName("org.nd4j.nativeblas.Nd4jCpu$Environment")
				Dim getInstance As System.Reflection.MethodInfo = clazz.GetMethod("getInstance")
				Dim instance As Object = getInstance.invoke(Nothing)
				Dim allowHelpers As System.Reflection.MethodInfo = clazz.GetMethod("allowHelpers", GetType(Boolean))
				allowHelpers.invoke(instance, False)
			Catch t As Exception
				Throw New Exception(t)
			End Try
		End Sub

		Public Shared Sub enableCppHelpers()
			Try
				Dim clazz As Type = DL4JClassLoading.loadClassByName("org.nd4j.nativeblas.Nd4jCpu$Environment")
				Dim getInstance As System.Reflection.MethodInfo = clazz.GetMethod("getInstance")
				Dim instance As Object = getInstance.invoke(Nothing)
				Dim allowHelpers As System.Reflection.MethodInfo = clazz.GetMethod("allowHelpers", GetType(Boolean))
				allowHelpers.invoke(instance, True)
			Catch t As Exception
				Throw New Exception(t)
			End Try
		End Sub

		Public Shared Sub validateMLN(ByVal netOrig As MultiLayerNetwork, ByVal t As TestCase)
			assertNotNull(t.getAllowHelpersForClasses())
			assertFalse(t.getAllowHelpersForClasses().isEmpty())

			'Don't allow fallback:
			For Each l As Layer In netOrig.Layers
				Dim lConf As org.deeplearning4j.nn.conf.layers.Layer = l.conf().getLayer()
				If TypeOf lConf Is ConvolutionLayer Then
					DirectCast(lConf, ConvolutionLayer).setCudnnAllowFallback(False)
				ElseIf TypeOf lConf Is SubsamplingLayer Then
					DirectCast(lConf, SubsamplingLayer).setCudnnAllowFallback(False)
				End If
			Next l


			Dim net1NoHelper As New MultiLayerNetwork(netOrig.LayerWiseConfigurations.clone())
			net1NoHelper.init()
			log.info("Removing all layer helpers from network copy 1")
			removeHelpers(net1NoHelper.Layers, Nothing)

			Dim net2With As New MultiLayerNetwork(netOrig.LayerWiseConfigurations.clone())
			net2With.init()
			net2With.params().assign(netOrig.params())
			log.info("Removing all except for specified helpers from network copy 2: " & t.getAllowHelpersForClasses())
			removeHelpers(net2With.Layers, t.getAllowHelpersForClasses())

			If t.isTestForward() Then
				Preconditions.checkNotNull(t.getFeatures(), "Features are not set (null)")

				For Each train As Boolean In New Boolean(){False, True}
					assertEquals(net1NoHelper.params(), net2With.params())
					Dim s As String = "Feed forward test - " & t.getTestName() & " - " & (If(train, "Train: ", "Test: "))
					Dim ff1 As IList(Of INDArray)
					Try
						disableCppHelpers()
						ff1 = net1NoHelper.feedForward(t.getFeatures(), train)
					Finally
						enableCppHelpers()
					End Try
					Dim ff2 As IList(Of INDArray) = net2With.feedForward(t.getFeatures(), train)
					Dim paramKeys As IList(Of String) = New List(Of String)(net1NoHelper.paramTable().Keys)
					paramKeys.Sort()
					For Each p As String In paramKeys
						Dim p1 As INDArray = net1NoHelper.getParam(p)
						Dim p2 As INDArray = net2With.getParam(p)
						Dim re As INDArray = relError(p1, p2, t.getMinAbsError())
						Dim maxRE As Double = re.maxNumber().doubleValue()
						If maxRE >= t.getMaxRelError() Then
							Console.WriteLine("Failed param values: parameter " & p & " - No heper vs. with helper - train=" & train)
							Console.WriteLine(p1)
							Console.WriteLine(p2)
						End If
						assertTrue(maxRE < t.getMaxRelError(),s & " - param changed during forward pass: " & p)
					Next p

					For i As Integer = 0 To ff1.Count - 1
						Dim layerIdx As Integer = i-1 'FF includes input
						Dim layerName As String = "layer_" & layerIdx & " - " & (If(i = 0, "input", net1NoHelper.getLayer(layerIdx).GetType().Name))
						Dim arr1 As INDArray = ff1(i)
						Dim arr2 As INDArray = ff2(i)

						Dim relError As INDArray = LayerHelperValidationUtil.relError(arr1, arr2, t.getMinAbsError())
						Dim maxRE As Double = relError.maxNumber().doubleValue()
						Dim idx As Integer = relError.argMax(Integer.MaxValue).getInt(0)
						If maxRE >= t.getMaxRelError() Then
							Dim d1 As Double = arr1.dup("c"c).getDouble(idx)
							Dim d2 As Double = arr2.dup("c"c).getDouble(idx)
							Console.WriteLine("Different values at index " & idx & ": " & d1 & ", " & d2 & " - RE = " & maxRE)
						End If
						assertTrue(maxRE < t.getMaxRelError(), s & layerName & " activations - max RE: " & maxRE)
						log.info("Forward pass, max relative error: " & layerName & " - " & maxRE)
					Next i

					Dim out1 As INDArray
					Try
						disableCppHelpers()
						out1 = net1NoHelper.output(t.getFeatures(), train)
					Finally
						enableCppHelpers()
					End Try
					Dim out2 As INDArray = net2With.output(t.getFeatures(), train)
					Dim relError As INDArray = LayerHelperValidationUtil.relError(out1, out2, t.getMinAbsError())
					Dim maxRE As Double = relError.maxNumber().doubleValue()
					log.info(s & "Output, max relative error: " & maxRE)

					assertEquals(net1NoHelper.params(), net2With.params()) 'Check that forward pass does not modify params
					assertTrue(maxRE < t.getMaxRelError(), s & "Max RE: " & maxRE)
				Next train
			End If


			If t.isTestScore() Then
				Preconditions.checkNotNull(t.getFeatures(), "Features are not set (null)")
				Preconditions.checkNotNull(t.getLabels(), "Labels are not set (null)")

				log.info("Validation - checking scores")
				Dim s1 As Double
				Try
					disableCppHelpers()
					s1 = net1NoHelper.score(New DataSet(t.getFeatures(), t.getLabels()))
				Finally
					enableCppHelpers()
				End Try
				Dim s2 As Double = net2With.score(New DataSet(t.getFeatures(), t.getLabels()))

				Dim re As Double = relError(s1, s2)
				Dim s As String = "Relative error: " & re
				assertTrue(re < t.getMaxRelError(), s)
			End If

			If t.isTestBackward() Then
				Preconditions.checkNotNull(t.getFeatures(), "Features are not set (null)")
				Preconditions.checkNotNull(t.getLabels(), "Labels are not set (null)")
				log.info("Validation - checking backward pass")

				'Check gradients
				net1NoHelper.Input = t.getFeatures()
				net1NoHelper.Labels = t.getLabels()

				net2With.Input = t.getFeatures()
				net2With.Labels = t.getLabels()

				Try
					disableCppHelpers()
					net1NoHelper.computeGradientAndScore()
				Finally
					enableCppHelpers()
				End Try
				net2With.computeGradientAndScore()

				Dim paramKeys As IList(Of String) = New List(Of String)(net1NoHelper.paramTable().Keys)
				paramKeys.Sort()
				For Each p As String In paramKeys
					Dim g1 As INDArray = net1NoHelper.gradient().gradientForVariable()(p)
					Dim g2 As INDArray = net2With.gradient().gradientForVariable()(p)

					If g1 Is Nothing OrElse g2 Is Nothing Then
						Throw New Exception("Null gradients")
					End If

					Dim re As INDArray = relError(g1, g2, t.getMinAbsError())
					Dim maxRE As Double = re.maxNumber().doubleValue()
					If maxRE >= t.getMaxRelError() Then
						Console.WriteLine("Failed param values: no helper vs. with helper - parameter: " & p)
						Console.WriteLine(java.util.Arrays.toString(g1.dup().data().asFloat()))
						Console.WriteLine(java.util.Arrays.toString(g2.dup().data().asFloat()))
					Else
						Console.WriteLine("OK: " & p)
					End If
					assertTrue(maxRE < t.getMaxRelError(), t.getTestName() & " - Gradients are not equal: " & p & " - highest relative error = " & maxRE & " > max relative error = " & t.getMaxRelError())
				Next p
			End If

			If t.isTestTraining() Then
				Preconditions.checkNotNull(t.getData(), "DataSetIterator is not set (null)")
				log.info("Testing run-to-run consistency of training with layer helper")

				net2With = New MultiLayerNetwork(netOrig.LayerWiseConfigurations.clone())
				net2With.init()
				net2With.params().assign(netOrig.params())
				log.info("Removing all except for specified layer helpers from network copy 2: " & t.getAllowHelpersForClasses())
				removeHelpers(net2With.Layers, t.getAllowHelpersForClasses())

				Dim listener As New CollectScoresListener(1)
				net2With.setListeners(listener)
				net2With.fit(t.getData())

				For i As Integer = 0 To 1

					net2With = New MultiLayerNetwork(netOrig.LayerWiseConfigurations.clone())
					net2With.init()
					net2With.params().assign(netOrig.params())
					log.info("Removing all except for specified layer helpers from network copy 2: " & t.getAllowHelpersForClasses())
					removeHelpers(net2With.Layers, t.getAllowHelpersForClasses())

					Dim listener2 As New CollectScoresListener(1)
					net2With.setListeners(listener2)
					net2With.fit(t.getData())

					Dim listOrig As DoubleArrayList = listener.getListScore()
					Dim listNew As DoubleArrayList = listener2.getListScore()

					assertEquals(listOrig.size(), listNew.size())
					For j As Integer = 0 To listOrig.size() - 1
						Dim d1 As Double = listOrig.get(j)
						Dim d2 As Double = listNew.get(j)
						Dim re As Double = relError(d1, d2)
						Dim msg As String = "Scores at iteration " & j & " - relError = " & re & ", score1 = " & d1 & ", score2 = " & d2
						assertTrue(re < t.getMaxRelError(), msg)
						Console.WriteLine("j=" & j & ", d1 = " & d1 & ", d2 = " & d2)
					Next j
				Next i
			End If
		End Sub

		Private Shared Sub removeHelpers(ByVal layers() As Layer, ByVal keepHelpersFor As IList(Of [Class]))

			Dim map As IDictionary(Of Type, Integer) = New Dictionary(Of Type, Integer)()
			For Each l As Layer In layers
				Dim f As System.Reflection.FieldInfo
				Try
					f = l.GetType().getDeclaredField("helper")
				Catch e As Exception
					'OK, may not be a layer helper supported layer
					Continue For
				End Try

				f.setAccessible(True)
				Dim keepAndAssertPresent As Boolean = False
				If keepHelpersFor IsNot Nothing Then
					For Each c As Type In keepHelpersFor
						If c.IsAssignableFrom(l.GetType()) Then
							keepAndAssertPresent = True
							Exit For
						End If
					Next c
				End If
				Try
					If keepAndAssertPresent Then
						Dim o As Object = f.get(l)
						assertNotNull(o,"Expect helper to be present for layer: " & l.GetType())
					Else
						f.set(l, Nothing)
						Dim i As Integer? = map(l.GetType())
						If i Is Nothing Then
							i = 0
						End If
						map(l.GetType()) = i.Value+1
					End If
				Catch e As IllegalAccessException
					Throw New Exception(e)
				End Try
			Next l

			For Each c As KeyValuePair(Of Type, Integer) In map.SetOfKeyValuePairs()
				Console.WriteLine("Removed " & c.Value & " layer helpers instances from layer " & c.Key)
			Next c
		End Sub

		Private Shared Function relError(ByVal d1 As Double, ByVal d2 As Double) As Double
			Preconditions.checkState(Not Double.IsNaN(d1), "d1 is NaN")
			Preconditions.checkState(Not Double.IsNaN(d2), "d2 is NaN")
			If d1 = 0.0 AndAlso d2 = 0.0 Then
				Return 0.0
			End If

			Return Math.Abs(d1-d2) / (Math.Abs(d1) + Math.Abs(d2))
		End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: private static org.nd4j.linalg.api.ndarray.INDArray relError(@NonNull INDArray a1, @NonNull INDArray a2, double minAbsError)
		Private Shared Function relError(ByVal a1 As INDArray, ByVal a2 As INDArray, ByVal minAbsError As Double) As INDArray
			Dim numNaN1 As Long = Nd4j.Executioner.exec(New MatchCondition(a1, Conditions.Nan, Integer.MaxValue)).getInt(0)
			Dim numNaN2 As Long = Nd4j.Executioner.exec(New MatchCondition(a2, Conditions.Nan, Integer.MaxValue)).getInt(0)
			Preconditions.checkState(numNaN1 = 0, "Array 1 has NaNs")
			Preconditions.checkState(numNaN2 = 0, "Array 2 has NaNs")

			Dim abs1 As INDArray = Transforms.abs(a1, True)
			Dim abs2 As INDArray = Transforms.abs(a2, True)
			Dim absDiff As INDArray = Transforms.abs(a1.sub(a2), False)

			'abs(a1-a2) < minAbsError ? 1 : 0
			Dim greaterThanMinAbs As INDArray = Transforms.abs(a1.sub(a2), False)
			BooleanIndexing.replaceWhere(greaterThanMinAbs, 0.0, Conditions.lessThan(minAbsError))
			BooleanIndexing.replaceWhere(greaterThanMinAbs, 1.0, Conditions.greaterThan(0.0))

			Dim result As INDArray = absDiff.divi(abs1.add(abs2))
			'Only way to have NaNs given there weren't any in original : both 0s
			BooleanIndexing.replaceWhere(result, 0.0, Conditions.Nan)
			'Finally, set to 0 if less than min abs error, or unchanged otherwise
			result.muli(greaterThanMinAbs)

			Return result
		End Function

	End Class

End Namespace