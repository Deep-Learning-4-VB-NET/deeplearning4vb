Imports System
Imports FileUtils = org.apache.commons.io.FileUtils
Imports BaseDL4JTest = org.deeplearning4j.BaseDL4JTest
Imports EarlyTerminationDataSetIterator = org.deeplearning4j.datasets.iterator.EarlyTerminationDataSetIterator
Imports MnistDataSetIterator = org.deeplearning4j.datasets.iterator.impl.MnistDataSetIterator
Imports MultiLayerConfiguration = org.deeplearning4j.nn.conf.MultiLayerConfiguration
Imports NeuralNetConfiguration = org.deeplearning4j.nn.conf.NeuralNetConfiguration
Imports NormalDistribution = org.deeplearning4j.nn.conf.distribution.NormalDistribution
Imports InputType = org.deeplearning4j.nn.conf.inputs.InputType
Imports ConvolutionLayer = org.deeplearning4j.nn.conf.layers.ConvolutionLayer
Imports OutputLayer = org.deeplearning4j.nn.conf.layers.OutputLayer
Imports PoolingType = org.deeplearning4j.nn.conf.layers.PoolingType
Imports SubsamplingLayer = org.deeplearning4j.nn.conf.layers.SubsamplingLayer
Imports ComputationGraph = org.deeplearning4j.nn.graph.ComputationGraph
Imports MultiLayerNetwork = org.deeplearning4j.nn.multilayer.MultiLayerNetwork
Imports ScoreIterationListener = org.deeplearning4j.optimize.listeners.ScoreIterationListener
Imports org.junit.jupiter.api
Imports TempDir = org.junit.jupiter.api.io.TempDir
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports Activation = org.nd4j.linalg.activations.Activation
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports DataSetIterator = org.nd4j.linalg.dataset.api.iterator.DataSetIterator
Imports NoOp = org.nd4j.linalg.learning.config.NoOp
Imports LossFunctions = org.nd4j.linalg.lossfunctions.LossFunctions
Imports org.junit.jupiter.api.Assertions
Imports ExtendWith = org.junit.jupiter.api.extension.ExtendWith

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
Namespace org.deeplearning4j.util

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @DisplayName("Crash Reporting Util Test") @NativeTag @Tag(TagNames.JAVA_ONLY) @Tag(TagNames.FILE_IO) class CrashReportingUtilTest extends org.deeplearning4j.BaseDL4JTest
	Friend Class CrashReportingUtilTest
		Inherits BaseDL4JTest

		Public Overrides ReadOnly Property TimeoutMilliseconds As Long
			Get
				Return 120000
			End Get
		End Property

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @TempDir public java.nio.file.Path testDir;
		Public testDir As Path

		Public Overrides ReadOnly Property DataType As DataType
			Get
				Return DataType.FLOAT
			End Get
		End Property

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @AfterEach void after()
		Friend Overridable Sub after()
			' Reset dir
			CrashReportingUtil.crashDumpOutputDirectory(Nothing)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test") @Disabled void test() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub test()
			Dim dir As File = testDir.toFile()
			CrashReportingUtil.crashDumpOutputDirectory(dir)
			Dim kernel As Integer = 2
			Dim stride As Integer = 1
			Dim padding As Integer = 0
			Dim poolingType As PoolingType = PoolingType.MAX
			Dim inputDepth As Integer = 1
			Dim height As Integer = 28
			Dim width As Integer = 28
			Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).updater(New NoOp()).dist(New NormalDistribution(0, 1)).list().layer(0, (New ConvolutionLayer.Builder()).kernelSize(kernel, kernel).stride(stride, stride).padding(padding, padding).nIn(inputDepth).nOut(3).build()).layer(1, (New SubsamplingLayer.Builder(poolingType)).kernelSize(kernel, kernel).stride(stride, stride).padding(padding, padding).build()).layer(2, (New OutputLayer.Builder(LossFunctions.LossFunction.MCXENT)).activation(Activation.SOFTMAX).nOut(10).build()).setInputType(InputType.convolutionalFlat(height, width, inputDepth)).build()
			Dim net As New MultiLayerNetwork(conf)
			net.init()
			net.addListeners(New ScoreIterationListener(1))
			' Test net that hasn't been trained yet
			Dim e As New Exception()
			CrashReportingUtil.writeMemoryCrashDump(net, e)
			Dim list() As File = dir.listFiles()
			assertNotNull(list)
			assertEquals(1, list.Length)
			Dim str As String = FileUtils.readFileToString(list(0))
			' System.out.println(str);
			assertTrue(str.Contains("Network Information"))
			assertTrue(str.Contains("Layer Helpers"))
			assertTrue(str.Contains("JavaCPP"))
			assertTrue(str.Contains("ScoreIterationListener"))
			' Train:
			Dim iter As DataSetIterator = New EarlyTerminationDataSetIterator(New MnistDataSetIterator(32, True, 12345), 5)
			net.fit(iter)
			dir = testDir.toFile()
			CrashReportingUtil.crashDumpOutputDirectory(dir)
			CrashReportingUtil.writeMemoryCrashDump(net, e)
			list = dir.listFiles()
			assertNotNull(list)
			assertEquals(1, list.Length)
			str = FileUtils.readFileToString(list(0))
			assertTrue(str.Contains("Network Information"))
			assertTrue(str.Contains("Layer Helpers"))
			assertTrue(str.Contains("JavaCPP"))
			assertTrue(str.Contains("ScoreIterationListener(1)"))
			' System.out.println("///////////////////////////////////////////////////////////");
			' System.out.println(str);
			' System.out.println("///////////////////////////////////////////////////////////");
			' Also test manual memory info
			Dim mlnMemoryInfo As String = net.memoryInfo(32, InputType.convolutionalFlat(28, 28, 1))
			' System.out.println("///////////////////////////////////////////////////////////");
			' System.out.println(mlnMemoryInfo);
			' System.out.println("///////////////////////////////////////////////////////////");
			assertTrue(mlnMemoryInfo.Contains("Network Information"))
			assertTrue(mlnMemoryInfo.Contains("Layer Helpers"))
			assertTrue(mlnMemoryInfo.Contains("JavaCPP"))
			assertTrue(mlnMemoryInfo.Contains("ScoreIterationListener(1)"))
			' //////////////////////////////////////
			' Same thing on ComputationGraph:
			dir = testDir.toFile()
			CrashReportingUtil.crashDumpOutputDirectory(dir)
			Dim cg As ComputationGraph = net.toComputationGraph()
			cg.setListeners(New ScoreIterationListener(1))
			' Test net that hasn't been trained yet
			CrashReportingUtil.writeMemoryCrashDump(cg, e)
			list = dir.listFiles()
			assertNotNull(list)
			assertEquals(1, list.Length)
			str = FileUtils.readFileToString(list(0))
			assertTrue(str.Contains("Network Information"))
			assertTrue(str.Contains("Layer Helpers"))
			assertTrue(str.Contains("JavaCPP"))
			assertTrue(str.Contains("ScoreIterationListener(1)"))
			' Train:
			cg.fit(iter)
			dir = testDir.toFile()
			CrashReportingUtil.crashDumpOutputDirectory(dir)
			CrashReportingUtil.writeMemoryCrashDump(cg, e)
			list = dir.listFiles()
			assertNotNull(list)
			assertEquals(1, list.Length)
			str = FileUtils.readFileToString(list(0))
			assertTrue(str.Contains("Network Information"))
			assertTrue(str.Contains("Layer Helpers"))
			assertTrue(str.Contains("JavaCPP"))
			assertTrue(str.Contains("ScoreIterationListener(1)"))
			' System.out.println("///////////////////////////////////////////////////////////");
			' System.out.println(str);
			' System.out.println("///////////////////////////////////////////////////////////");
			' Also test manual memory info
			Dim cgMemoryInfo As String = cg.memoryInfo(32, InputType.convolutionalFlat(28, 28, 1))
			' System.out.println("///////////////////////////////////////////////////////////");
			' System.out.println(cgMemoryInfo);
			' System.out.println("///////////////////////////////////////////////////////////");
			assertTrue(cgMemoryInfo.Contains("Network Information"))
			assertTrue(cgMemoryInfo.Contains("Layer Helpers"))
			assertTrue(cgMemoryInfo.Contains("JavaCPP"))
			assertTrue(cgMemoryInfo.Contains("ScoreIterationListener(1)"))
		End Sub
	End Class

End Namespace