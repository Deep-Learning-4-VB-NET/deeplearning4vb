Imports System
Imports System.Collections.Concurrent
Imports System.Collections.Generic
Imports Microsoft.VisualBasic
Imports SerializerInstance = org.apache.spark.serializer.SerializerInstance
Imports ComputationGraphConfiguration = org.deeplearning4j.nn.conf.ComputationGraphConfiguration
Imports MultiLayerConfiguration = org.deeplearning4j.nn.conf.MultiLayerConfiguration
Imports NeuralNetConfiguration = org.deeplearning4j.nn.conf.NeuralNetConfiguration
Imports UniformDistribution = org.deeplearning4j.nn.conf.distribution.UniformDistribution
Imports org.deeplearning4j.nn.conf.graph
Imports DuplicateToTimeSeriesVertex = org.deeplearning4j.nn.conf.graph.rnn.DuplicateToTimeSeriesVertex
Imports LastTimeStepVertex = org.deeplearning4j.nn.conf.graph.rnn.LastTimeStepVertex
Imports org.deeplearning4j.nn.conf.layers
Imports CnnToFeedForwardPreProcessor = org.deeplearning4j.nn.conf.preprocessor.CnnToFeedForwardPreProcessor
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports org.nd4j.evaluation
Imports org.nd4j.evaluation.classification
Imports RegressionEvaluation = org.nd4j.evaluation.regression.RegressionEvaluation
Imports Activation = org.nd4j.linalg.activations.Activation
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports Adam = org.nd4j.linalg.learning.config.Adam
Imports Nadam = org.nd4j.linalg.learning.config.Nadam
Imports MapSchedule = org.nd4j.linalg.schedule.MapSchedule
Imports ScheduleType = org.nd4j.linalg.schedule.ScheduleType
Imports JavaConversions = scala.collection.JavaConversions
import static org.junit.jupiter.api.Assertions.assertTrue

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

Namespace org.deeplearning4j.spark


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Tag(TagNames.FILE_IO) @Tag(TagNames.SPARK) @Tag(TagNames.DIST_SYSTEMS) @NativeTag public class TestKryo extends BaseSparkKryoTest
	<Serializable>
	Public Class TestKryo
		Inherits BaseSparkKryoTest

		Private Sub testSerialization(Of T)(ByVal [in] As T, ByVal si As SerializerInstance)
			Dim bb As ByteBuffer = si.serialize([in], Nothing)
			Dim deserialized As T = CType(si.deserialize(bb, Nothing), T)

			Dim equals As Boolean = [in].Equals(deserialized)
			assertTrue(equals, [in].GetType() & vbTab & [in].ToString())
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testSerializationConfigurations()
		Public Overridable Sub testSerializationConfigurations()

			Dim si As SerializerInstance = sc.env().serializer().newInstance()

			'Check network configurations:
			Dim m As IDictionary(Of Integer, Double) = New Dictionary(Of Integer, Double)()
			m(0) = 0.5
			m(10) = 0.1
			Dim mlc As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).updater(New Nadam(New MapSchedule(ScheduleType.ITERATION,m))).list().layer(0, (New OutputLayer.Builder()).nIn(10).nOut(10).build()).build()

			testSerialization(mlc, si)


			Dim cgc As ComputationGraphConfiguration = (New NeuralNetConfiguration.Builder()).dist(New UniformDistribution(-1, 1)).updater(New Adam(New MapSchedule(ScheduleType.ITERATION,m))).graphBuilder().addInputs("in").addLayer("out", (New OutputLayer.Builder()).nIn(10).nOut(10).build(), "in").setOutputs("out").build()

			testSerialization(cgc, si)


			'Check main layers:
			Dim layers() As Layer = {(New OutputLayer.Builder()).nIn(10).nOut(10).build(), (New RnnOutputLayer.Builder()).nIn(10).nOut(10).build(), (New LossLayer.Builder()).build(), (New CenterLossOutputLayer.Builder()).nIn(10).nOut(10).build(), (New DenseLayer.Builder()).nIn(10).nOut(10).build(), (New ConvolutionLayer.Builder()).nIn(10).nOut(10).build(), (New SubsamplingLayer.Builder()).build(), (New Convolution1DLayer.Builder(2, 2)).nIn(10).nOut(10).build(), (New ActivationLayer.Builder()).activation(Activation.TANH).build(), (New GlobalPoolingLayer.Builder()).build(), (New GravesLSTM.Builder()).nIn(10).nOut(10).build(), (New LSTM.Builder()).nIn(10).nOut(10).build(), (New DropoutLayer.Builder(0.5)).build(), (New BatchNormalization.Builder()).build(), (New LocalResponseNormalization.Builder()).build()}

			For Each l As Layer In layers
				testSerialization(l, si)
			Next l

			'Check graph vertices
			Dim vertices() As GraphVertex = {
				New ElementWiseVertex(ElementWiseVertex.Op.Add),
				New L2NormalizeVertex(),
				New LayerVertex(Nothing, Nothing),
				New MergeVertex(),
				New PoolHelperVertex(),
				New PreprocessorVertex(New CnnToFeedForwardPreProcessor(28, 28, 1)),
				New ReshapeVertex(New Integer() {1, 1}),
				New ScaleVertex(1.0),
				New ShiftVertex(1.0),
				New SubsetVertex(1, 1),
				New UnstackVertex(0, 2),
				New DuplicateToTimeSeriesVertex("in1"),
				New LastTimeStepVertex("in1")
			}

			For Each gv As GraphVertex In vertices
				testSerialization(gv, si)
			Next gv
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testSerializationEvaluation()
		Public Overridable Sub testSerializationEvaluation()

			Dim e As New Evaluation()
			e.eval(Nd4j.create(New Double() {1, 0, 0}, New Long(){1, 3}), Nd4j.create(New Double() {0.2, 0.5, 0.3}, New Long(){1, 3}))

			Dim eb As New EvaluationBinary()
			eb.eval(Nd4j.create(New Double() {1, 0, 0}, New Long(){1, 3}), Nd4j.create(New Double() {0.2, 0.6, 0.3}, New Long(){1, 3}))

			Dim roc As New ROC(30)
			roc.eval(Nd4j.create(New Double() {1}, New Long(){1, 1}), Nd4j.create(New Double() {0.2}, New Long(){1, 1}))
			Dim roc2 As New ROC()
			roc2.eval(Nd4j.create(New Double() {1}, New Long(){1, 1}), Nd4j.create(New Double() {0.2}, New Long(){1, 1}))

			Dim rocM As New ROCMultiClass(30)
			rocM.eval(Nd4j.create(New Double() {1, 0, 0}, New Long(){1, 3}), Nd4j.create(New Double() {0.2, 0.5, 0.3}, New Long(){1, 3}))
			Dim rocM2 As New ROCMultiClass()
			rocM2.eval(Nd4j.create(New Double() {1, 0, 0}, New Long(){1, 3}), Nd4j.create(New Double() {0.2, 0.5, 0.3}, New Long(){1, 3}))

			Dim rocB As New ROCBinary(30)
			rocB.eval(Nd4j.create(New Double() {1, 0, 0}, New Long(){1, 3}), Nd4j.create(New Double() {0.2, 0.6, 0.3}, New Long(){1, 3}))

			Dim rocB2 As New ROCBinary()
			rocB2.eval(Nd4j.create(New Double() {1, 0, 0}, New Long(){1, 3}), Nd4j.create(New Double() {0.2, 0.6, 0.3}, New Long(){1, 3}))

			Dim re As New RegressionEvaluation()
			re.eval(Nd4j.rand(1, 5), Nd4j.rand(1, 5))

			Dim evaluations() As IEvaluation = {
				New Evaluation(),
				e,
				New EvaluationBinary(),
				eb,
				New ROC(),
				roc,
				roc2,
				New ROCMultiClass(),
				rocM,
				rocM2,
				New ROCBinary(),
				rocB,
				rocB2,
				New RegressionEvaluation(),
				re
			}

			Dim si As SerializerInstance = sc.env().serializer().newInstance()

			For Each ie As IEvaluation In evaluations
				'System.out.println(ie.getClass());
				testSerialization(ie, si)
			Next ie
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testScalaCollections()
		Public Overridable Sub testScalaCollections()
			'Scala collections should already work with Spark + kryo; some very basic tests to check this is still the case
			Dim si As SerializerInstance = sc.env().serializer().newInstance()

			Dim emptyImmutableMap As scala.collection.immutable.Map(Of Integer, String) = scala.collection.immutable.Map$.MODULE$.empty()
			testSerialization(emptyImmutableMap, si)

			Dim m As IDictionary(Of Integer, Double) = New Dictionary(Of Integer, Double)()
			m(0) = 1.0

			Dim m2 As scala.collection.Map(Of Integer, Double) = JavaConversions.mapAsScalaMap(m)
			testSerialization(m2, si)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testJavaTypes()
		Public Overridable Sub testJavaTypes()

			Dim m As IDictionary(Of Object, Object) = New Dictionary(Of Object, Object)()
			m("key") = "value"

			Dim si As SerializerInstance = sc.env().serializer().newInstance()

			testSerialization(Collections.singletonMap("key", "value"), si)
			testSerialization(Collections.synchronizedMap(m), si)
			testSerialization(java.util.Collections.emptyMap(), si)
			testSerialization(New ConcurrentDictionary(Of )(m), si)
			testSerialization(Collections.unmodifiableMap(m), si)

			testSerialization(java.util.Arrays.asList("s"), si)
			testSerialization(Collections.singleton("s"), si)
			testSerialization(Collections.synchronizedList(java.util.Arrays.asList("s")), si)
			testSerialization(java.util.Collections.emptyList(), si)
			testSerialization(New CopyOnWriteArrayList(Of )(java.util.Arrays.asList("s")), si)
			testSerialization(Collections.unmodifiableList(java.util.Arrays.asList("s")), si)

			testSerialization(Collections.singleton("s"), si)
			testSerialization(Collections.synchronizedSet(New HashSet(Of )(java.util.Arrays.asList("s"))), si)
			testSerialization(java.util.Collections.emptySet(), si)
			testSerialization(Collections.unmodifiableSet(New HashSet(Of )(java.util.Arrays.asList("s"))), si)
		End Sub
	End Class

End Namespace