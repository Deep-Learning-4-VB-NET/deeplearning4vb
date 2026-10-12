Imports SparkConf = org.apache.spark.SparkConf
Imports JavaSparkContext = org.apache.spark.api.java.JavaSparkContext
Imports ComputationGraphConfiguration = org.deeplearning4j.nn.conf.ComputationGraphConfiguration
Imports MultiLayerConfiguration = org.deeplearning4j.nn.conf.MultiLayerConfiguration
Imports NeuralNetConfiguration = org.deeplearning4j.nn.conf.NeuralNetConfiguration
Imports OutputLayer = org.deeplearning4j.nn.conf.layers.OutputLayer
Imports org.deeplearning4j.spark.api
Imports SparkComputationGraph = org.deeplearning4j.spark.impl.graph.SparkComputationGraph
Imports SparkDl4jMultiLayer = org.deeplearning4j.spark.impl.multilayer.SparkDl4jMultiLayer
Imports ParameterAveragingTrainingMaster = org.deeplearning4j.spark.impl.paramavg.ParameterAveragingTrainingMaster
Imports Disabled = org.junit.jupiter.api.Disabled
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames

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

Namespace org.deeplearning4j.spark.impl

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Tag(TagNames.FILE_IO) @Tag(TagNames.SPARK) @Tag(TagNames.DIST_SYSTEMS) @NativeTag public class TestKryoWarning
	Public Class TestKryoWarning

		Private Shared Sub doTestMLN(ByVal sparkConf As SparkConf)
			Dim sc As New JavaSparkContext(sparkConf)

			Try

				Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).list().layer(0, (New OutputLayer.Builder()).nIn(10).nOut(10).build()).build()

				Dim tm As TrainingMaster = (New ParameterAveragingTrainingMaster.Builder(1)).build()

				Dim sml As New SparkDl4jMultiLayer(sc, conf, tm)
			Finally
				sc.stop()
			End Try
		End Sub

		Private Shared Sub doTestCG(ByVal sparkConf As SparkConf)
			Dim sc As New JavaSparkContext(sparkConf)

			Try

				Dim conf As ComputationGraphConfiguration = (New NeuralNetConfiguration.Builder()).graphBuilder().addInputs("in").addLayer("0", (New OutputLayer.Builder()).nIn(10).nOut(10).build(), "in").setOutputs("0").build()

				Dim tm As TrainingMaster = (New ParameterAveragingTrainingMaster.Builder(1)).build()

				Dim scg As SparkListenable = New SparkComputationGraph(sc, conf, tm)
			Finally
				sc.stop()
			End Try
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @Disabled public void testKryoMessageMLNIncorrectConfig()
		Public Overridable Sub testKryoMessageMLNIncorrectConfig()
			'Should print warning message
			Dim sparkConf As SparkConf = (New SparkConf()).setMaster("local[*]").setAppName("sparktest").set("spark.driver.host", "localhost").set("spark.serializer", "org.apache.spark.serializer.KryoSerializer")

			doTestMLN(sparkConf)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @Disabled public void testKryoMessageMLNCorrectConfigKryo()
		Public Overridable Sub testKryoMessageMLNCorrectConfigKryo()
			'Should NOT print warning message
			Dim sparkConf As SparkConf = (New SparkConf()).setMaster("local[*]").setAppName("sparktest").set("spark.driver.host", "localhost").set("spark.serializer", "org.apache.spark.serializer.KryoSerializer").set("spark.kryo.registrator", "org.nd4j.kryo.Nd4jRegistrator")

			doTestMLN(sparkConf)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @Disabled public void testKryoMessageMLNCorrectConfigNoKryo()
		Public Overridable Sub testKryoMessageMLNCorrectConfigNoKryo()
			'Should NOT print warning message
			Dim sparkConf As SparkConf = (New SparkConf()).setMaster("local[*]").set("spark.driver.host", "localhost").setAppName("sparktest")

			doTestMLN(sparkConf)
		End Sub



'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @Disabled public void testKryoMessageCGIncorrectConfig()
		Public Overridable Sub testKryoMessageCGIncorrectConfig()
			'Should print warning message
			Dim sparkConf As SparkConf = (New SparkConf()).setMaster("local[*]").setAppName("sparktest").set("spark.driver.host", "localhost").set("spark.serializer", "org.apache.spark.serializer.KryoSerializer")

			doTestCG(sparkConf)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @Disabled public void testKryoMessageCGCorrectConfigKryo()
		Public Overridable Sub testKryoMessageCGCorrectConfigKryo()
			'Should NOT print warning message
			Dim sparkConf As SparkConf = (New SparkConf()).setMaster("local[*]").setAppName("sparktest").set("spark.driver.host", "localhost").set("spark.serializer", "org.apache.spark.serializer.KryoSerializer").set("spark.kryo.registrator", "org.nd4j.kryo.Nd4jRegistrator")

			doTestCG(sparkConf)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @Disabled public void testKryoMessageCGCorrectConfigNoKryo()
		Public Overridable Sub testKryoMessageCGCorrectConfigNoKryo()
			'Should NOT print warning message
			Dim sparkConf As SparkConf = (New SparkConf()).setMaster("local[*]").set("spark.driver.host", "localhost").setAppName("sparktest")

			doTestCG(sparkConf)
		End Sub

	End Class

End Namespace