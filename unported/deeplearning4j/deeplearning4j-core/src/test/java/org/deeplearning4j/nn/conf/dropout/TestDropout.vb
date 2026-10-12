Imports System
Imports System.Collections.Generic
Imports Data = lombok.Data
Imports BaseDL4JTest = org.deeplearning4j.BaseDL4JTest
Imports TestUtils = org.deeplearning4j.TestUtils
Imports ExistingDataSetIterator = org.deeplearning4j.datasets.iterator.ExistingDataSetIterator
Imports ComputationGraphConfiguration = org.deeplearning4j.nn.conf.ComputationGraphConfiguration
Imports MultiLayerConfiguration = org.deeplearning4j.nn.conf.MultiLayerConfiguration
Imports NeuralNetConfiguration = org.deeplearning4j.nn.conf.NeuralNetConfiguration
Imports LayerVertex = org.deeplearning4j.nn.conf.graph.LayerVertex
Imports DenseLayer = org.deeplearning4j.nn.conf.layers.DenseLayer
Imports DropoutLayer = org.deeplearning4j.nn.conf.layers.DropoutLayer
Imports OutputLayer = org.deeplearning4j.nn.conf.layers.OutputLayer
Imports ComputationGraph = org.deeplearning4j.nn.graph.ComputationGraph
Imports MultiLayerNetwork = org.deeplearning4j.nn.multilayer.MultiLayerNetwork
Imports LayerWorkspaceMgr = org.deeplearning4j.nn.workspace.LayerWorkspaceMgr
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports MatchCondition = org.nd4j.linalg.api.ops.impl.reduce.longer.MatchCondition
Imports DataSet = org.nd4j.linalg.dataset.DataSet
Imports DataSetIterator = org.nd4j.linalg.dataset.api.iterator.DataSetIterator
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports Conditions = org.nd4j.linalg.indexing.conditions.Conditions
Imports LossFunctions = org.nd4j.linalg.lossfunctions.LossFunctions
Imports org.nd4j.common.primitives
Imports MapSchedule = org.nd4j.linalg.schedule.MapSchedule
Imports ScheduleType = org.nd4j.linalg.schedule.ScheduleType
import static org.junit.jupiter.api.Assertions.assertEquals
import static org.junit.jupiter.api.Assertions.assertTrue
import static org.nd4j.linalg.indexing.NDArrayIndex.all
import static org.nd4j.linalg.indexing.NDArrayIndex.point

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

Namespace org.deeplearning4j.nn.conf.dropout


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @NativeTag @Tag(TagNames.DL4J_OLD_API) public class TestDropout extends org.deeplearning4j.BaseDL4JTest
	Public Class TestDropout
		Inherits BaseDL4JTest

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testBasicConfig()
		Public Overridable Sub testBasicConfig()

			Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).dropOut(0.6).list().layer((New DenseLayer.Builder()).nIn(10).nOut(10).build()).layer((New DenseLayer.Builder()).nIn(10).nOut(10).dropOut(0.7).build()).layer((New DenseLayer.Builder()).nIn(10).nOut(10).dropOut(New AlphaDropout(0.5)).build()).build()

			assertEquals(New Dropout(0.6), conf.getConf(0).getLayer().getIDropout())
			assertEquals(New Dropout(0.7), conf.getConf(1).getLayer().getIDropout())
			assertEquals(New AlphaDropout(0.5), conf.getConf(2).getLayer().getIDropout())


			Dim conf2 As ComputationGraphConfiguration = (New NeuralNetConfiguration.Builder()).dropOut(0.6).graphBuilder().addInputs("in").addLayer("0", (New DenseLayer.Builder()).nIn(10).nOut(10).build(), "in").addLayer("1", (New DenseLayer.Builder()).nIn(10).nOut(10).dropOut(0.7).build(), "0").addLayer("2", (New DenseLayer.Builder()).nIn(10).nOut(10).dropOut(New AlphaDropout(0.5)).build(), "1").setOutputs("2").build()

			assertEquals(New Dropout(0.6), CType(conf2.getVertices().get("0"), LayerVertex).getLayerConf().getLayer().getIDropout())
			assertEquals(New Dropout(0.7), CType(conf2.getVertices().get("1"), LayerVertex).getLayerConf().getLayer().getIDropout())
			assertEquals(New AlphaDropout(0.5), CType(conf2.getVertices().get("2"), LayerVertex).getLayerConf().getLayer().getIDropout())
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testCalls()
		Public Overridable Sub testCalls()

			Dim d1 As New CustomDropout()
			Dim d2 As New CustomDropout()

			Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).list().layer((New DenseLayer.Builder()).nIn(4).nOut(3).dropOut(d1).build()).layer((New OutputLayer.Builder(LossFunctions.LossFunction.MSE)).dropOut(d2).nIn(3).nOut(3).build()).build()
			Dim net As New MultiLayerNetwork(conf)
			net.init()

			Dim l As IList(Of DataSet) = New List(Of DataSet)()
			l.Add(New DataSet(Nd4j.rand(5,4), Nd4j.rand(5,3)))
			l.Add(New DataSet(Nd4j.rand(5,4), Nd4j.rand(5,3)))
			l.Add(New DataSet(Nd4j.rand(5,4), Nd4j.rand(5,3)))

			Dim iter As DataSetIterator = New ExistingDataSetIterator(l)

			net.fit(iter)
			net.fit(iter)

			Dim expList As IList(Of Pair(Of Integer, Integer)) = New List(Of Pair(Of Integer, Integer)) From {
				New Pair(Of Pair(Of Integer, Integer))(0, 0),
				New Pair(Of )(1, 0),
				New Pair(Of )(2, 0),
				New Pair(Of )(3, 1),
				New Pair(Of )(4, 1),
				New Pair(Of )(5, 1)
			}

			assertEquals(expList, d1.getAllCalls())
			assertEquals(expList, d2.getAllCalls())

			assertEquals(expList, d1.getAllReverseCalls())
			assertEquals(expList, d2.getAllReverseCalls())


			d1 = New CustomDropout()
			d2 = New CustomDropout()
			Dim conf2 As ComputationGraphConfiguration = (New NeuralNetConfiguration.Builder()).graphBuilder().addInputs("in").addLayer("0", (New DenseLayer.Builder()).nIn(4).nOut(3).dropOut(d1).build(), "in").addLayer("1", (New OutputLayer.Builder(LossFunctions.LossFunction.MSE)).dropOut(d2).nIn(3).nOut(3).build(), "0").setOutputs("1").build()

			Dim net2 As New ComputationGraph(conf2)
			net2.init()

			net2.fit(iter)
			net2.fit(iter)

			assertEquals(expList, d1.getAllCalls())
			assertEquals(expList, d2.getAllCalls())
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Data public static class CustomDropout implements IDropout
		<Serializable>
		Public Class CustomDropout
			Implements IDropout

			Friend allCalls As IList(Of Pair(Of Integer, Integer)) = New List(Of Pair(Of Integer, Integer))()
			Friend allReverseCalls As IList(Of Pair(Of Integer, Integer)) = New List(Of Pair(Of Integer, Integer))()

			Public Overridable Function applyDropout(ByVal inputActivations As INDArray, ByVal result As INDArray, ByVal iteration As Integer, ByVal epoch As Integer, ByVal workspaceMgr As LayerWorkspaceMgr) As INDArray Implements IDropout.applyDropout
				allCalls.Add(New Pair(Of Integer, Integer)(iteration, epoch))
				Return inputActivations
			End Function

			Public Overridable Function backprop(ByVal gradAtOutput As INDArray, ByVal gradAtInput As INDArray, ByVal iteration As Integer, ByVal epoch As Integer) As INDArray Implements IDropout.backprop
				allReverseCalls.Add(New Pair(Of Integer, Integer)(iteration, epoch))
				Return gradAtInput
			End Function

			Public Overridable Sub clear() Implements IDropout.clear

			End Sub

			Public Overridable Function clone() As IDropout Implements IDropout.clone
				Return Me
			End Function
		End Class

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testSerialization()
		Public Overridable Sub testSerialization()

			Dim dropouts() As IDropout = {
				New Dropout(0.5),
				New AlphaDropout(0.5),
				New GaussianDropout(0.1),
				New GaussianNoise(0.1)
			}

			For Each id As IDropout In dropouts

				Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).dropOut(id).list().layer((New DenseLayer.Builder()).nIn(4).nOut(3).build()).layer((New OutputLayer.Builder(LossFunctions.LossFunction.MSE)).nIn(3).nOut(3).build()).build()
				Dim net As New MultiLayerNetwork(conf)
				net.init()

				TestUtils.testModelSerialization(net)

				Dim conf2 As ComputationGraphConfiguration = (New NeuralNetConfiguration.Builder()).dropOut(id).graphBuilder().addInputs("in").addLayer("0", (New DenseLayer.Builder()).nIn(4).nOut(3).build(), "in").addLayer("1", (New OutputLayer.Builder(LossFunctions.LossFunction.MSE)).nIn(3).nOut(3).build(), "0").setOutputs("1").build()

				Dim net2 As New ComputationGraph(conf2)
				net2.init()

				TestUtils.testModelSerialization(net2)
			Next id
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testDropoutValues()
		Public Overridable Sub testDropoutValues()
			Nd4j.Random.setSeed(12345)

			Dim d As New Dropout(0.5)

			Dim [in] As INDArray = Nd4j.ones(10, 10)
			Dim [out] As INDArray = d.applyDropout([in], Nd4j.create(10,10), 0, 0, LayerWorkspaceMgr.noWorkspacesImmutable())

			assertEquals([in], Nd4j.ones(10, 10))

			Dim countZeros As Integer = Nd4j.Executioner.exec(New MatchCondition([out], Conditions.equals(0))).getInt(0)
			Dim countTwos As Integer = Nd4j.Executioner.exec(New MatchCondition([out], Conditions.equals(2))).getInt(0)

			assertEquals(100, countZeros + countTwos) 'Should only be 0 or 2
			'Stochastic, but this should hold for most cases
			assertTrue(countZeros >= 25 AndAlso countZeros <= 75)
			assertTrue(countTwos >= 25 AndAlso countTwos <= 75)

			'Test schedule:
			d = New Dropout((New MapSchedule.Builder(ScheduleType.ITERATION)).add(0, 0.5).add(5, 0.1).build())
			For i As Integer = 0 To 9
				[out] = d.applyDropout([in], Nd4j.create([in].shape()), i, 0, LayerWorkspaceMgr.noWorkspacesImmutable())
				assertEquals([in], Nd4j.ones(10, 10))
				countZeros = Nd4j.Executioner.exec(New MatchCondition([out], Conditions.equals(0))).getInt(0)

				If i < 5 Then
					countTwos = Nd4j.Executioner.exec(New MatchCondition([out], Conditions.equals(2))).getInt(0)
					assertEquals(100, countZeros + countTwos,i.ToString()) 'Should only be 0 or 2
					'Stochastic, but this should hold for most cases
					assertTrue(countZeros >= 25 AndAlso countZeros <= 75)
					assertTrue(countTwos >= 25 AndAlso countTwos <= 75)
				Else
					Dim countInverse As Integer = Nd4j.Executioner.exec(New MatchCondition([out], Conditions.equals(1.0/0.1))).getInt(0)
					assertEquals(100, countZeros + countInverse) 'Should only be 0 or 10
					'Stochastic, but this should hold for most cases
					assertTrue(countZeros >= 80)
					assertTrue(countInverse <= 20)
				End If
			Next i
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testGaussianDropoutValues()
		Public Overridable Sub testGaussianDropoutValues()
			Nd4j.Random.setSeed(12345)

			Dim d As New GaussianDropout(0.1) 'sqrt(0.1/(1-0.1)) = 0.3333 stdev

			Dim [in] As INDArray = Nd4j.ones(50, 50)
			Dim [out] As INDArray = d.applyDropout([in], Nd4j.create([in].shape()), 0, 0, LayerWorkspaceMgr.noWorkspacesImmutable())

			assertEquals([in], Nd4j.ones(50, 50))

			Dim mean As Double = [out].meanNumber().doubleValue()
			Dim stdev As Double = [out].stdNumber().doubleValue()

			assertEquals(1.0, mean, 0.05)
			assertEquals(0.333, stdev, 0.02)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testGaussianNoiseValues()
		Public Overridable Sub testGaussianNoiseValues()
			Nd4j.Random.setSeed(12345)

			Dim d As New GaussianNoise(0.1) 'sqrt(0.1/(1-0.1)) = 0.3333 stdev

			Dim [in] As INDArray = Nd4j.ones(50, 50)
			Dim [out] As INDArray = d.applyDropout([in], Nd4j.create([in].shape()), 0, 0, LayerWorkspaceMgr.noWorkspacesImmutable())

			assertEquals([in], Nd4j.ones(50, 50))

			Dim mean As Double = [out].meanNumber().doubleValue()
			Dim stdev As Double = [out].stdNumber().doubleValue()

			assertEquals(1.0, mean, 0.05)
			assertEquals(0.1, stdev, 0.01)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testAlphaDropoutValues()
		Public Overridable Sub testAlphaDropoutValues()
			Nd4j.Random.setSeed(12345)

			Dim p As Double = 0.4
			Dim d As New AlphaDropout(p)

			Dim SELU_ALPHA As Double = 1.6732632423543772
			Dim SELU_LAMBDA As Double = 1.0507009873554804
			Dim alphaPrime As Double = - SELU_LAMBDA * SELU_ALPHA
			Dim a As Double = 1.0 / Math.Sqrt((p + alphaPrime * alphaPrime * p * (1-p)))
			Dim b As Double = -1.0 / Math.Sqrt(p + alphaPrime * alphaPrime * p * (1-p)) * (1-p) * alphaPrime

			Dim actA As Double = d.a(p)
			Dim actB As Double = d.b(p)

			assertEquals(a, actA, 1e-6)
			assertEquals(b, actB, 1e-6)

			Dim [in] As INDArray = Nd4j.ones(10, 10)
			Dim [out] As INDArray = d.applyDropout([in], Nd4j.create([in].shape()), 0, 0, LayerWorkspaceMgr.noWorkspacesImmutable())

			Dim countValueDropped As Integer = 0
			Dim countEqn As Integer = 0
			Dim eqn As Double = a * 1 + b
			Dim valueDropped As Double = a * alphaPrime + b
			For i As Integer = 0 To 99
				Dim v As Double = [out].getDouble(i)
				If v >= valueDropped - 1e-6 AndAlso v <= valueDropped + 1e-6 Then
					countValueDropped += 1
				ElseIf v >= eqn - 1e-6 AndAlso v <= eqn + 1e-6 Then
					countEqn += 1
				End If

			Next i

			assertEquals(100, countValueDropped + countEqn)
			assertTrue(countValueDropped >= 25 AndAlso countValueDropped <= 75)
			assertTrue(countEqn >= 25 AndAlso countEqn <= 75)
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testSpatialDropout5DValues()
		Public Overridable Sub testSpatialDropout5DValues()
			Nd4j.Random.setSeed(12345)

			Dim d As New SpatialDropout(0.5)

			Dim [in] As INDArray = Nd4j.ones(10, 10, 5, 5, 5)
			Dim [out] As INDArray = d.applyDropout([in], Nd4j.create([in].shape()), 0, 0, LayerWorkspaceMgr.noWorkspacesImmutable())

			assertEquals([in], Nd4j.ones(10, 10, 5, 5, 5))

			'Now, we expect all values for a given depth to be the same... 0 or 2
			Dim countZero As Integer = 0
			Dim countTwo As Integer = 0
			For i As Integer = 0 To 9
				For j As Integer = 0 To 9
					Dim value As Double = [out].getDouble(i,j,0,0,0)
					assertTrue(value = 0 OrElse value = 2.0)
					Dim exp As INDArray = Nd4j.valueArrayOf(New Integer(){5, 5, 5}, value)
					Dim act As INDArray = [out].get(point(i), point(j), all(), all(),all())
					assertEquals(exp, act)

					If value = 0.0 Then
						countZero += 1
					Else
						countTwo += 1
					End If
				Next j
			Next i

			'Stochastic, but this should hold for most cases
			assertTrue(countZero >= 25 AndAlso countZero <= 75)
			assertTrue(countTwo >= 25 AndAlso countTwo <= 75)

			'Test schedule:
			d = New SpatialDropout((New MapSchedule.Builder(ScheduleType.ITERATION)).add(0, 0.5).add(5, 0.1).build())
			For i As Integer = 0 To 9
				[out] = d.applyDropout([in], Nd4j.create([in].shape()), i, 0, LayerWorkspaceMgr.noWorkspacesImmutable())
				assertEquals([in], Nd4j.ones(10, 10, 5, 5, 5))

				If i < 5 Then
					countZero = 0
					countTwo = 0
					For m As Integer = 0 To 9
						For j As Integer = 0 To 9
							Dim value As Double = [out].getDouble(m,j,0,0,0)
							assertTrue(value = 0 OrElse value = 2.0)
							Dim exp As INDArray = Nd4j.valueArrayOf(New Integer(){5, 5, 5}, value)
							Dim act As INDArray = [out].get(point(m), point(j), all(), all(), all())
							assertEquals(exp, act)

							If value = 0.0 Then
								countZero += 1
							Else
								countTwo += 1
							End If
						Next j
					Next m

					'Stochastic, but this should hold for most cases
					assertTrue(countZero >= 25 AndAlso countZero <= 75)
					assertTrue(countTwo >= 25 AndAlso countTwo <= 75)
				Else
					countZero = 0
					Dim countInverse As Integer = 0
					For m As Integer = 0 To 9
						For j As Integer = 0 To 9
							Dim value As Double = [out].getDouble(m,j,0,0,0)
							assertTrue(value = 0 OrElse value = 10.0)
							Dim exp As INDArray = Nd4j.valueArrayOf(New Integer(){5, 5, 5}, value)
							Dim act As INDArray = [out].get(point(m), point(j), all(), all(),all())
							assertEquals(exp, act)

							If value = 0.0 Then
								countZero += 1
							Else
								countInverse += 1
							End If
						Next j
					Next m

					'Stochastic, but this should hold for most cases
					assertTrue(countZero >= 80)
					assertTrue(countInverse <= 20)
				End If
			Next i
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testSpatialDropoutValues()
		Public Overridable Sub testSpatialDropoutValues()
			Nd4j.Random.setSeed(12345)

			Dim d As New SpatialDropout(0.5)

			Dim [in] As INDArray = Nd4j.ones(10, 10, 5, 5)
			Dim [out] As INDArray = d.applyDropout([in], Nd4j.create([in].shape()), 0, 0, LayerWorkspaceMgr.noWorkspacesImmutable())

			assertEquals([in], Nd4j.ones(10, 10, 5, 5))

			'Now, we expect all values for a given depth to be the same... 0 or 2
			Dim countZero As Integer = 0
			Dim countTwo As Integer = 0
			For i As Integer = 0 To 9
				For j As Integer = 0 To 9
					Dim value As Double = [out].getDouble(i,j,0,0)
					assertTrue(value = 0 OrElse value = 2.0)
					Dim exp As INDArray = Nd4j.valueArrayOf(New Integer(){5, 5}, value)
					Dim act As INDArray = [out].get(point(i), point(j), all(), all())
					assertEquals(exp, act)

					If value = 0.0 Then
						countZero += 1
					Else
						countTwo += 1
					End If
				Next j
			Next i

			'Stochastic, but this should hold for most cases
			assertTrue(countZero >= 25 AndAlso countZero <= 75)
			assertTrue(countTwo >= 25 AndAlso countTwo <= 75)

			'Test schedule:
			d = New SpatialDropout((New MapSchedule.Builder(ScheduleType.ITERATION)).add(0, 0.5).add(5, 0.1).build())
			For i As Integer = 0 To 9
				[out] = d.applyDropout([in], Nd4j.create([in].shape()), i, 0, LayerWorkspaceMgr.noWorkspacesImmutable())
				assertEquals([in], Nd4j.ones(10, 10, 5, 5))

				If i < 5 Then
					countZero = 0
					countTwo = 0
					For m As Integer = 0 To 9
						For j As Integer = 0 To 9
							Dim value As Double = [out].getDouble(m,j,0,0)
							assertTrue(value = 0 OrElse value = 2.0)
							Dim exp As INDArray = Nd4j.valueArrayOf(New Integer(){5, 5}, value)
							Dim act As INDArray = [out].get(point(m), point(j), all(), all())
							assertEquals(exp, act)

							If value = 0.0 Then
								countZero += 1
							Else
								countTwo += 1
							End If
						Next j
					Next m

					'Stochastic, but this should hold for most cases
					assertTrue(countZero >= 25 AndAlso countZero <= 75)
					assertTrue(countTwo >= 25 AndAlso countTwo <= 75)
				Else
					countZero = 0
					Dim countInverse As Integer = 0
					For m As Integer = 0 To 9
						For j As Integer = 0 To 9
							Dim value As Double = [out].getDouble(m,j,0,0)
							assertTrue(value = 0 OrElse value = 10.0)
							Dim exp As INDArray = Nd4j.valueArrayOf(New Integer(){5, 5}, value)
							Dim act As INDArray = [out].get(point(m), point(j), all(), all())
							assertEquals(exp, act)

							If value = 0.0 Then
								countZero += 1
							Else
								countInverse += 1
							End If
						Next j
					Next m

					'Stochastic, but this should hold for most cases
					assertTrue(countZero >= 80)
					assertTrue(countInverse <= 20)
				End If
			Next i
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testSpatialDropoutValues3D()
		Public Overridable Sub testSpatialDropoutValues3D()
			Nd4j.Random.setSeed(12345)

			Dim d As New SpatialDropout(0.5)

			Dim [in] As INDArray = Nd4j.ones(10, 8, 12)
			Dim [out] As INDArray = d.applyDropout([in], Nd4j.create([in].shape()), 0, 0, LayerWorkspaceMgr.noWorkspacesImmutable())

			assertEquals([in], Nd4j.ones(10, 8, 12))

			'Now, we expect all values for a given depth to be the same... 0 or 2
			Dim countZero As Integer = 0
			Dim countTwo As Integer = 0
			For i As Integer = 0 To 9
				For j As Integer = 0 To 7
					Dim value As Double = [out].getDouble(i,j,0)
					assertTrue(value = 0 OrElse value = 2.0)
					Dim exp As INDArray = Nd4j.valueArrayOf(New Integer(){12}, value)
					Dim act As INDArray = [out].get(point(i), point(j), all())
					assertEquals(exp, act)

					If value = 0.0 Then
						countZero += 1
					Else
						countTwo += 1
					End If
				Next j
			Next i

			'Stochastic, but this should hold for most cases
			assertTrue(countZero >= 20 AndAlso countZero <= 60)
			assertTrue(countTwo >= 20 AndAlso countTwo <= 60)

			'Test schedule:
			d = New SpatialDropout((New MapSchedule.Builder(ScheduleType.ITERATION)).add(0, 0.5).add(5, 0.1).build())
			For i As Integer = 0 To 9
				[out] = d.applyDropout([in], Nd4j.create([in].shape()), i, 0, LayerWorkspaceMgr.noWorkspacesImmutable())
				assertEquals([in], Nd4j.ones(10, 8, 12))

				If i < 5 Then
					countZero = 0
					countTwo = 0
					For m As Integer = 0 To 9
						For j As Integer = 0 To 7
							Dim value As Double = [out].getDouble(m,j,0)
							assertTrue(value = 0 OrElse value = 2.0)
							Dim exp As INDArray = Nd4j.valueArrayOf(New Integer(){12}, value)
							Dim act As INDArray = [out].get(point(m), point(j), all())
							assertEquals(exp, act)

							If value = 0.0 Then
								countZero += 1
							Else
								countTwo += 1
							End If
						Next j
					Next m

					'Stochastic, but this should hold for most cases
					assertTrue(countZero >= 20 AndAlso countZero <= 60)
					assertTrue(countTwo >= 20 AndAlso countTwo <= 60)
				Else
					countZero = 0
					Dim countInverse As Integer = 0
					For m As Integer = 0 To 9
						For j As Integer = 0 To 7
							Dim value As Double = [out].getDouble(m,j,0)
							assertTrue(value = 0 OrElse value = 10.0)
							Dim exp As INDArray = Nd4j.valueArrayOf(New Integer(){12}, value)
							Dim act As INDArray = [out].get(point(m), point(j), all())
							assertEquals(exp, act)

							If value = 0.0 Then
								countZero += 1
							Else
								countInverse += 1
							End If
						Next j
					Next m

					'Stochastic, but this should hold for most cases
					assertTrue(countZero >= 60)
					assertTrue(countInverse <= 15)
				End If
			Next i
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testSpatialDropoutJSON()
		Public Overridable Sub testSpatialDropoutJSON()

			Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).list().layer((New DropoutLayer.Builder(New SpatialDropout(0.5))).build()).build()

			Dim asJson As String = conf.toJson()
			Dim fromJson As MultiLayerConfiguration = MultiLayerConfiguration.fromJson(asJson)

			assertEquals(conf, fromJson)
		End Sub

	End Class

End Namespace