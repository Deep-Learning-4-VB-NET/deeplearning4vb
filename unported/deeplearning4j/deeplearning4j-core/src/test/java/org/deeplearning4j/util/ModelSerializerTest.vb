Imports System
Imports System.Collections.Generic
Imports System.IO
Imports val = lombok.val
Imports SerializationUtils = org.apache.commons.lang3.SerializationUtils
Imports BaseDL4JTest = org.deeplearning4j.BaseDL4JTest
Imports IrisDataSetIterator = org.deeplearning4j.datasets.iterator.impl.IrisDataSetIterator
Imports OptimizationAlgorithm = org.deeplearning4j.nn.api.OptimizationAlgorithm
Imports ComputationGraphConfiguration = org.deeplearning4j.nn.conf.ComputationGraphConfiguration
Imports MultiLayerConfiguration = org.deeplearning4j.nn.conf.MultiLayerConfiguration
Imports NeuralNetConfiguration = org.deeplearning4j.nn.conf.NeuralNetConfiguration
Imports DenseLayer = org.deeplearning4j.nn.conf.layers.DenseLayer
Imports OutputLayer = org.deeplearning4j.nn.conf.layers.OutputLayer
Imports ComputationGraph = org.deeplearning4j.nn.graph.ComputationGraph
Imports MultiLayerNetwork = org.deeplearning4j.nn.multilayer.MultiLayerNetwork
Imports WeightInit = org.deeplearning4j.nn.weights.WeightInit
Imports Disabled = org.junit.jupiter.api.Disabled
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports TempDir = org.junit.jupiter.api.io.TempDir
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports Activation = org.nd4j.linalg.activations.Activation
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports DataSet = org.nd4j.linalg.dataset.DataSet
Imports DataSetIterator = org.nd4j.linalg.dataset.api.iterator.DataSetIterator
Imports org.nd4j.linalg.dataset.api.preprocessor
Imports NormalizerMinMaxScaler = org.nd4j.linalg.dataset.api.preprocessor.NormalizerMinMaxScaler
Imports NormalizerStandardize = org.nd4j.linalg.dataset.api.preprocessor.NormalizerStandardize
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports Sgd = org.nd4j.linalg.learning.config.Sgd
Imports LossFunctions = org.nd4j.linalg.lossfunctions.LossFunctions
Imports org.nd4j.common.primitives
Imports org.junit.jupiter.api.Assertions
Imports DisplayName = org.junit.jupiter.api.DisplayName
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
'ORIGINAL LINE: @DisplayName("Model Serializer Test") @Disabled @NativeTag @Tag(TagNames.FILE_IO) class ModelSerializerTest extends org.deeplearning4j.BaseDL4JTest
	Friend Class ModelSerializerTest
		Inherits BaseDL4JTest

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @TempDir public java.nio.file.Path tempDir;
		Public tempDir As Path

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Write MLN Model") void testWriteMLNModel() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub testWriteMLNModel()
			Dim nIn As Integer = 5
			Dim nOut As Integer = 6
			Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).seed(12345).l1(0.01).l2(0.01).updater(New Sgd(0.1)).activation(Activation.TANH).weightInit(WeightInit.XAVIER).list().layer(0, (New DenseLayer.Builder()).nIn(nIn).nOut(20).build()).layer(1, (New DenseLayer.Builder()).nIn(20).nOut(30).build()).layer(2, (New OutputLayer.Builder()).lossFunction(LossFunctions.LossFunction.MSE).nIn(30).nOut(nOut).build()).build()
			Dim net As New MultiLayerNetwork(conf)
			net.init()
			Dim tempFile As File = tempDir.toFile()
			ModelSerializer.writeModel(net, tempFile, True)
			Dim network As MultiLayerNetwork = ModelSerializer.restoreMultiLayerNetwork(tempFile)
			assertEquals(network.LayerWiseConfigurations.toJson(), net.LayerWiseConfigurations.toJson())
			assertEquals(net.params(), network.params())
			assertEquals(net.Updater.StateViewArray, network.Updater.StateViewArray)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Write Mln Model Input Stream") void testWriteMlnModelInputStream() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub testWriteMlnModelInputStream()
			Dim nIn As Integer = 5
			Dim nOut As Integer = 6
			Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).seed(12345).l1(0.01).l2(0.01).updater(New Sgd(0.1)).activation(Activation.TANH).weightInit(WeightInit.XAVIER).list().layer(0, (New DenseLayer.Builder()).nIn(nIn).nOut(20).build()).layer(1, (New DenseLayer.Builder()).nIn(20).nOut(30).build()).layer(2, (New OutputLayer.Builder()).lossFunction(LossFunctions.LossFunction.MSE).nIn(30).nOut(nOut).build()).build()
			Dim net As New MultiLayerNetwork(conf)
			net.init()
			Dim tempFile As File = tempDir.toFile()
			Dim fos As New FileStream(tempFile, FileMode.Create, FileAccess.Write)
			ModelSerializer.writeModel(net, fos, True)
			' checking adding of DataNormalization to the model file
			Dim scaler As New NormalizerMinMaxScaler()
			Dim iter As DataSetIterator = New IrisDataSetIterator(150, 150)
			scaler.fit(iter)
			ModelSerializer.addNormalizerToModel(tempFile, scaler)
			Dim restoredScaler As NormalizerMinMaxScaler = ModelSerializer.restoreNormalizerFromFile(tempFile)
			assertNotEquals(Nothing, scaler.Max)
			assertEquals(scaler.Max, restoredScaler.Max)
			assertEquals(scaler.Min, restoredScaler.Min)
			Dim fis As New FileStream(tempFile, FileMode.Open, FileAccess.Read)
			Dim network As MultiLayerNetwork = ModelSerializer.restoreMultiLayerNetwork(fis)
			assertEquals(network.LayerWiseConfigurations.toJson(), net.LayerWiseConfigurations.toJson())
			assertEquals(net.params(), network.params())
			assertEquals(net.Updater.StateViewArray, network.Updater.StateViewArray)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Write CG Model") void testWriteCGModel() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub testWriteCGModel()
			Dim config As ComputationGraphConfiguration = (New NeuralNetConfiguration.Builder()).optimizationAlgo(OptimizationAlgorithm.STOCHASTIC_GRADIENT_DESCENT).updater(New Sgd(0.1)).graphBuilder().addInputs("in").addLayer("dense", (New DenseLayer.Builder()).nIn(4).nOut(2).build(), "in").addLayer("out", (New OutputLayer.Builder(LossFunctions.LossFunction.MCXENT)).nIn(2).nOut(3).activation(Activation.SOFTMAX).build(), "dense").setOutputs("out").build()
			Dim cg As New ComputationGraph(config)
			cg.init()
			Dim tempFile As File = tempDir.toFile()
			ModelSerializer.writeModel(cg, tempFile, True)
			Dim network As ComputationGraph = ModelSerializer.restoreComputationGraph(tempFile)
			assertEquals(network.Configuration.toJson(), cg.Configuration.toJson())
			assertEquals(cg.params(), network.params())
			assertEquals(cg.Updater.StateViewArray, network.Updater.StateViewArray)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Write CG Model Input Stream") void testWriteCGModelInputStream() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub testWriteCGModelInputStream()
			Dim config As ComputationGraphConfiguration = (New NeuralNetConfiguration.Builder()).optimizationAlgo(OptimizationAlgorithm.STOCHASTIC_GRADIENT_DESCENT).updater(New Sgd(0.1)).graphBuilder().addInputs("in").addLayer("dense", (New DenseLayer.Builder()).nIn(4).nOut(2).build(), "in").addLayer("out", (New OutputLayer.Builder(LossFunctions.LossFunction.MCXENT)).nIn(2).nOut(3).activation(Activation.SOFTMAX).build(), "dense").setOutputs("out").build()
			Dim cg As New ComputationGraph(config)
			cg.init()
			Dim tempFile As File = tempDir.toFile()
			ModelSerializer.writeModel(cg, tempFile, True)
			Dim fis As New FileStream(tempFile, FileMode.Open, FileAccess.Read)
			Dim network As ComputationGraph = ModelSerializer.restoreComputationGraph(fis)
			assertEquals(network.Configuration.toJson(), cg.Configuration.toJson())
			assertEquals(cg.params(), network.params())
			assertEquals(cg.Updater.StateViewArray, network.Updater.StateViewArray)
		End Sub

		Private Function trivialDataSet() As DataSet
			Dim inputs As INDArray = Nd4j.create(New Single() { 1.0f, 2.0f, 3.0f }, New Integer() { 1, 3 })
			Dim labels As INDArray = Nd4j.create(New Single() { 4.0f, 5.0f, 6.0f }, New Integer() { 1, 3 })
			Return New DataSet(inputs, labels)
		End Function

		Private Function simpleComputationGraph() As ComputationGraph
			Dim config As ComputationGraphConfiguration = (New NeuralNetConfiguration.Builder()).optimizationAlgo(OptimizationAlgorithm.STOCHASTIC_GRADIENT_DESCENT).updater(New Sgd(0.1)).graphBuilder().addInputs("in").addLayer("dense", (New DenseLayer.Builder()).nIn(4).nOut(2).build(), "in").addLayer("out", (New OutputLayer.Builder(LossFunctions.LossFunction.MCXENT)).nIn(2).nOut(3).activation(Activation.SOFTMAX).build(), "dense").setOutputs("out").build()
			Return New ComputationGraph(config)
		End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Save Restore Normalizer From Input Stream") void testSaveRestoreNormalizerFromInputStream() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub testSaveRestoreNormalizerFromInputStream()
			Dim dataSet As DataSet = trivialDataSet()
			Dim norm As New NormalizerStandardize()
			norm.fit(dataSet)
			Dim cg As ComputationGraph = simpleComputationGraph()
			cg.init()
			Dim tempFile As File = tempDir.toFile()
			ModelSerializer.writeModel(cg, tempFile, True)
			ModelSerializer.addNormalizerToModel(tempFile, norm)
			Dim fis As New FileStream(tempFile, FileMode.Open, FileAccess.Read)
			Dim restored As NormalizerStandardize = ModelSerializer.restoreNormalizerFromInputStream(fis)
			assertNotEquals(Nothing, restored)
			Dim dataSet2 As DataSet = dataSet.copy()
			norm.preProcess(dataSet2)
			assertNotEquals(dataSet.Features, dataSet2.Features)
			restored.revert(dataSet2)
			assertEquals(dataSet.Features, dataSet2.Features)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Restore Unsaved Normalizer From Input Stream") void testRestoreUnsavedNormalizerFromInputStream() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub testRestoreUnsavedNormalizerFromInputStream()
			Dim dataSet As DataSet = trivialDataSet()
			Dim norm As New NormalizerStandardize()
			norm.fit(dataSet)
			Dim cg As ComputationGraph = simpleComputationGraph()
			cg.init()
			Dim tempFile As File = tempDir.toFile()
			ModelSerializer.writeModel(cg, tempFile, True)
			Dim fis As New FileStream(tempFile, FileMode.Open, FileAccess.Read)
			Dim restored As NormalizerStandardize = ModelSerializer.restoreNormalizerFromInputStream(fis)
			assertEquals(Nothing, restored)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Invalid Loading 1") void testInvalidLoading1() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub testInvalidLoading1()
			Dim config As ComputationGraphConfiguration = (New NeuralNetConfiguration.Builder()).graphBuilder().addInputs("in").addLayer("dense", (New DenseLayer.Builder()).nIn(4).nOut(2).build(), "in").addLayer("out", (New OutputLayer.Builder(LossFunctions.LossFunction.MCXENT)).activation(Activation.SOFTMAX).nIn(2).nOut(3).build(), "dense").setOutputs("out").build()
			Dim cg As New ComputationGraph(config)
			cg.init()
			Dim tempFile As File = tempDir.toFile()
			ModelSerializer.writeModel(cg, tempFile, True)
			Try
				ModelSerializer.restoreMultiLayerNetwork(tempFile)
				fail()
			Catch e As Exception
				Dim msg As String = e.Message
				assertTrue(msg.Contains("JSON") AndAlso msg.Contains("restoreComputationGraph"),msg)
			End Try
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Invalid Loading 2") void testInvalidLoading2() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub testInvalidLoading2()
			Dim nIn As Integer = 5
			Dim nOut As Integer = 6
			Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).seed(12345).l1(0.01).l2(0.01).updater(New Sgd(0.1)).activation(Activation.TANH).weightInit(WeightInit.XAVIER).list().layer(0, (New DenseLayer.Builder()).nIn(nIn).nOut(20).build()).layer(1, (New DenseLayer.Builder()).nIn(20).nOut(30).build()).layer(2, (New OutputLayer.Builder()).lossFunction(LossFunctions.LossFunction.MSE).nIn(30).nOut(nOut).build()).build()
			Dim net As New MultiLayerNetwork(conf)
			net.init()
			Dim tempFile As File = tempDir.resolve("testInvalidLoading2.bin").toFile()
			ModelSerializer.writeModel(net, tempFile, True)
			Try
				ModelSerializer.restoreComputationGraph(tempFile)
				fail()
			Catch e As Exception
				Dim msg As String = e.Message
				assertTrue(msg.Contains("JSON") AndAlso msg.Contains("restoreMultiLayerNetwork"),msg)
			End Try
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Invalid Stream Reuse") void testInvalidStreamReuse() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub testInvalidStreamReuse()
			Dim nIn As Integer = 5
			Dim nOut As Integer = 6
			Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).seed(12345).l1(0.01).list().layer((New OutputLayer.Builder()).nIn(nIn).nOut(nOut).activation(Activation.SOFTMAX).build()).build()
			Dim net As New MultiLayerNetwork(conf)
			net.init()
			Dim dataSet As DataSet = trivialDataSet()
			Dim norm As New NormalizerStandardize()
			norm.fit(dataSet)
			Dim tempFile As File = tempDir.toFile()
			ModelSerializer.writeModel(net, tempFile, True)
			ModelSerializer.addNormalizerToModel(tempFile, norm)
			Dim [is] As Stream = New FileStream(tempFile, FileMode.Open, FileAccess.Read)
			ModelSerializer.restoreMultiLayerNetwork([is])
			Try
				ModelSerializer.restoreNormalizerFromInputStream([is])
				fail("Expected exception")
			Catch e As Exception
				Dim msg As String = e.Message
				assertTrue(msg.Contains("may have been closed"),msg)
			End Try
			Try
				ModelSerializer.restoreMultiLayerNetwork([is])
				fail("Expected exception")
			Catch e As Exception
				Dim msg As String = e.Message
				assertTrue(msg.Contains("may have been closed"),msg)
			End Try
			' Also test reading  both model and normalizer from stream (correctly)
			Dim pair As Pair(Of MultiLayerNetwork, Normalizer) = ModelSerializer.restoreMultiLayerNetworkAndNormalizer(New FileStream(tempFile, FileMode.Open, FileAccess.Read), True)
			assertEquals(net.params(), pair.First.params())
			assertNotNull(pair.Second)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Invalid Stream Reuse CG") void testInvalidStreamReuseCG() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub testInvalidStreamReuseCG()
			Dim nIn As Integer = 5
			Dim nOut As Integer = 6
			Dim conf As ComputationGraphConfiguration = (New NeuralNetConfiguration.Builder()).seed(12345).l1(0.01).graphBuilder().addInputs("in").layer("0", (New OutputLayer.Builder()).nIn(nIn).nOut(nOut).activation(Activation.SOFTMAX).build(), "in").setOutputs("0").build()
			Dim net As New ComputationGraph(conf)
			net.init()
			Dim dataSet As DataSet = trivialDataSet()
			Dim norm As New NormalizerStandardize()
			norm.fit(dataSet)
			Dim tempFile As File = tempDir.toFile()
			ModelSerializer.writeModel(net, tempFile, True)
			ModelSerializer.addNormalizerToModel(tempFile, norm)
			Dim [is] As Stream = New FileStream(tempFile, FileMode.Open, FileAccess.Read)
			ModelSerializer.restoreComputationGraph([is])
			Try
				ModelSerializer.restoreNormalizerFromInputStream([is])
				fail("Expected exception")
			Catch e As Exception
				Dim msg As String = e.Message
				assertTrue(msg.Contains("may have been closed"),msg)
			End Try
			Try
				ModelSerializer.restoreComputationGraph([is])
				fail("Expected exception")
			Catch e As Exception
				Dim msg As String = e.Message
				assertTrue(msg.Contains("may have been closed"),msg)
			End Try
			' Also test reading  both model and normalizer from stream (correctly)
			Dim pair As Pair(Of ComputationGraph, Normalizer) = ModelSerializer.restoreComputationGraphAndNormalizer(New FileStream(tempFile, FileMode.Open, FileAccess.Read), True)
			assertEquals(net.params(), pair.First.params())
			assertNotNull(pair.Second)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Java Serde _ 1") void testJavaSerde_1() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub testJavaSerde_1()
			Dim nIn As Integer = 5
			Dim nOut As Integer = 6
			Dim conf As ComputationGraphConfiguration = (New NeuralNetConfiguration.Builder()).seed(12345).l1(0.01).graphBuilder().addInputs("in").layer("0", (New OutputLayer.Builder()).nIn(nIn).nOut(nOut).build(), "in").setOutputs("0").validateOutputLayerConfig(False).build()
			Dim net As New ComputationGraph(conf)
			net.init()
			Dim dataSet As DataSet = trivialDataSet()
			Dim norm As New NormalizerStandardize()
			norm.fit(dataSet)
			Dim b As val = SerializationUtils.serialize(net)
			Dim restored As ComputationGraph = SerializationUtils.deserialize(b)
			assertEquals(net, restored)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Java Serde _ 2") void testJavaSerde_2() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub testJavaSerde_2()
			Dim nIn As Integer = 5
			Dim nOut As Integer = 6
			Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).seed(12345).l1(0.01).list().layer(0, (New OutputLayer.Builder()).nIn(nIn).nOut(nOut).activation(Activation.SOFTMAX).build()).build()
			Dim net As New MultiLayerNetwork(conf)
			net.init()
			Dim dataSet As DataSet = trivialDataSet()
			Dim norm As New NormalizerStandardize()
			norm.fit(dataSet)
			Dim b As val = SerializationUtils.serialize(net)
			Dim restored As MultiLayerNetwork = SerializationUtils.deserialize(b)
			assertEquals(net, restored)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Put Get Object") void testPutGetObject() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub testPutGetObject()
			Dim nIn As Integer = 5
			Dim nOut As Integer = 6
			Dim conf As ComputationGraphConfiguration = (New NeuralNetConfiguration.Builder()).seed(12345).l1(0.01).graphBuilder().addInputs("in").layer("0", (New OutputLayer.Builder()).nIn(nIn).nOut(nOut).activation(Activation.SOFTMAX).build(), "in").setOutputs("0").build()
			Dim net As New ComputationGraph(conf)
			net.init()
			Dim tempFile As File = tempDir.toFile()
			ModelSerializer.writeModel(net, tempFile, True)
			Dim toWrite As IList(Of String) = New List(Of String) From {"zero", "one", "two"}
			ModelSerializer.addObjectToFile(tempFile, "myLabels", toWrite)
			Dim restored As IList(Of String) = ModelSerializer.getObjectFromFile(tempFile, "myLabels")
			assertEquals(toWrite, restored)
			Dim someOtherData As IDictionary(Of String, Object) = New Dictionary(Of String, Object)()
			someOtherData("x") = New Single() { 0, 1, 2 }
			someOtherData("y") = Nd4j.linspace(1, 10, 10, Nd4j.dataType())
			ModelSerializer.addObjectToFile(tempFile, "otherData.bin", someOtherData)
			Dim dataRestored As IDictionary(Of String, Object) = ModelSerializer.getObjectFromFile(tempFile, "otherData.bin")
			assertEquals(someOtherData.Keys, dataRestored.Keys)
			assertArrayEquals(DirectCast(someOtherData("x"), Single()), DirectCast(dataRestored("x"), Single()), 0f)
			assertEquals(someOtherData("y"), dataRestored("y"))
			Dim entries As IList(Of String) = ModelSerializer.listObjectsInFile(tempFile)
			assertEquals(2, entries.Count)
			Console.WriteLine(entries)
			assertTrue(entries.Contains("myLabels"))
			assertTrue(entries.Contains("otherData.bin"))
			Dim restoredNet As ComputationGraph = ModelSerializer.restoreComputationGraph(tempFile)
			assertEquals(net.params(), restoredNet.params())
		End Sub
	End Class

End Namespace