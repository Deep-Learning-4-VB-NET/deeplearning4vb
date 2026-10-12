Imports System
Imports System.Collections.Generic
Imports Platform = com.sun.jna.Platform
Imports SneakyThrows = lombok.SneakyThrows
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports Configuration = org.apache.hadoop.conf.Configuration
Imports SparkConf = org.apache.spark.SparkConf
Imports JavaRDD = org.apache.spark.api.java.JavaRDD
Imports JavaSparkContext = org.apache.spark.api.java.JavaSparkContext
Imports SerializableHadoopConfig = org.datavec.spark.util.SerializableHadoopConfig
Imports BaseDL4JTest = org.deeplearning4j.BaseDL4JTest
Imports MultiLayerConfiguration = org.deeplearning4j.nn.conf.MultiLayerConfiguration
Imports NeuralNetConfiguration = org.deeplearning4j.nn.conf.NeuralNetConfiguration
Imports SparkDl4jMultiLayer = org.deeplearning4j.spark.impl.multilayer.SparkDl4jMultiLayer
Imports ParameterAveragingTrainingMaster = org.deeplearning4j.spark.impl.paramavg.ParameterAveragingTrainingMaster
Imports AfterEach = org.junit.jupiter.api.AfterEach
Imports BeforeAll = org.junit.jupiter.api.BeforeAll
Imports BeforeEach = org.junit.jupiter.api.BeforeEach
Imports Downloader = org.nd4j.common.resources.Downloader
Imports Activation = org.nd4j.linalg.activations.Activation
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports DataSet = org.nd4j.linalg.dataset.DataSet
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports Nesterovs = org.nd4j.linalg.learning.config.Nesterovs
Imports LossFunctions = org.nd4j.linalg.lossfunctions.LossFunctions

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
'ORIGINAL LINE: @Slf4j public abstract class BaseSparkTest extends org.deeplearning4j.BaseDL4JTest implements java.io.Serializable
	<Serializable>
	Public MustInherit Class BaseSparkTest
		Inherits BaseDL4JTest

		<NonSerialized>
		Protected Friend sc As JavaSparkContext
		<NonSerialized>
		Protected Friend labels As INDArray
		<NonSerialized>
		Protected Friend input As INDArray
		<NonSerialized>
		Protected Friend rowSums As INDArray
		<NonSerialized>
		Protected Friend nRows As Integer = 200
		<NonSerialized>
		Protected Friend nIn As Integer = 4
		<NonSerialized>
		Protected Friend nOut As Integer = 3
		<NonSerialized>
		Protected Friend data As DataSet
		<NonSerialized>
		Protected Friend sparkData As JavaRDD(Of DataSet)

		Public Overrides ReadOnly Property TimeoutMilliseconds As Long
			Get
				Return 120000L
			End Get
		End Property
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @BeforeAll @SneakyThrows public static void beforeAll()
		Public Shared Sub beforeAll()
			If Platform.isWindows() Then
				Dim hadoopHome As New File(System.getProperty("java.io.tmpdir"),"hadoop-tmp")
				Dim binDir As New File(hadoopHome,"bin")
				If Not binDir.exists() Then
					binDir.mkdirs()
				End If
				Dim outputFile As New File(binDir,"winutils.exe")
				If Not outputFile.exists() Then
					log.info("Fixing spark for windows")
					Downloader.download("winutils.exe", URI.create("https://github.com/cdarlint/winutils/blob/master/hadoop-2.6.5/bin/winutils.exe?raw=true").toURL(), outputFile,"db24b404d2331a1bec7443336a5171f1",3)
				End If

				System.setProperty("hadoop.home.dir", hadoopHome.getAbsolutePath())
			End If
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @BeforeEach public void before()
		Public Overridable Sub before()

			sc = Context
			Dim r As New Random(12345)
			labels = Nd4j.create(nRows, nOut)
			input = Nd4j.rand(nRows, nIn)
			rowSums = input.sum(1)
			input.diviColumnVector(rowSums)

			For i As Integer = 0 To nRows - 1
				Dim x1 As Integer = r.Next(nOut)
				labels.putScalar(New Integer() {i, x1}, 1.0)
			Next i



			sparkData = getBasicSparkDataSet(nRows, input, labels)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @AfterEach public void after()
		Public Overridable Sub after()
			If sc IsNot Nothing Then
				sc.close()
			End If
			sc = Nothing
		End Sub

		''' 
		''' <summary>
		''' @return
		''' </summary>
		Public Overridable ReadOnly Property Context As JavaSparkContext
			Get
				If sc IsNot Nothing Then
					Return sc
				End If
				' set to test mode
				Dim sparkConf As SparkConf = (New SparkConf()).setMaster("local[" & numExecutors() & "]").set("spark.driver.host", "localhost").setAppName("sparktest")
    
    
				sc = New JavaSparkContext(sparkConf)
    
				Return sc
			End Get
		End Property

		Protected Friend Overridable Function getBasicSparkDataSet(ByVal nRows As Integer, ByVal input As INDArray, ByVal labels As INDArray) As JavaRDD(Of DataSet)
			Dim list As IList(Of DataSet) = New List(Of DataSet)()
			For i As Integer = 0 To nRows - 1
				Dim inRow As INDArray = input.getRow(i, True).dup()
				Dim outRow As INDArray = labels.getRow(i, True).dup()

				Dim ds As New DataSet(inRow, outRow)
				list.Add(ds)
			Next i
			list.GetEnumerator()

			data = DataSet.merge(list)
			Return sc.parallelize(list)
		End Function


		Protected Friend Overridable ReadOnly Property BasicNetwork As SparkDl4jMultiLayer
			Get
				Return New SparkDl4jMultiLayer(sc, BasicConf, New ParameterAveragingTrainingMaster(True, numExecutors(), 1, 10, 1, 0))
			End Get
		End Property

		Protected Friend Overridable Function numExecutors() As Integer
			Return 4
		End Function

		Protected Friend Overridable ReadOnly Property BasicConf As MultiLayerConfiguration
			Get
				Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).seed(123).updater(New Nesterovs(0.1, 0.9)).list().layer(0, (New org.deeplearning4j.nn.conf.layers.DenseLayer.Builder()).nIn(nIn).nOut(3).activation(Activation.TANH).build()).layer(1, (New org.deeplearning4j.nn.conf.layers.OutputLayer.Builder(LossFunctions.LossFunction.MCXENT)).nIn(3).nOut(nOut).activation(Activation.SOFTMAX).build()).build()
    
				Return conf
			End Get
		End Property


	End Class

End Namespace