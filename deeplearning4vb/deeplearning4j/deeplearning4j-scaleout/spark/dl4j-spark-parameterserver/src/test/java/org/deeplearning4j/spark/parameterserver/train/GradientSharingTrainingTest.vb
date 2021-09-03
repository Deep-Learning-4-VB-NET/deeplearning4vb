Imports System
Imports System.Collections.Concurrent
Imports System.Collections.Generic
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports JavaRDD = org.apache.spark.api.java.JavaRDD
Imports EarlyTerminationDataSetIterator = org.deeplearning4j.datasets.iterator.EarlyTerminationDataSetIterator
Imports IrisDataSetIterator = org.deeplearning4j.datasets.iterator.impl.IrisDataSetIterator
Imports MnistDataSetIterator = org.deeplearning4j.datasets.iterator.impl.MnistDataSetIterator
Imports Evaluation = org.deeplearning4j.eval.Evaluation
Imports Model = org.deeplearning4j.nn.api.Model
Imports ComputationGraphConfiguration = org.deeplearning4j.nn.conf.ComputationGraphConfiguration
Imports MultiLayerConfiguration = org.deeplearning4j.nn.conf.MultiLayerConfiguration
Imports NeuralNetConfiguration = org.deeplearning4j.nn.conf.NeuralNetConfiguration
Imports DenseLayer = org.deeplearning4j.nn.conf.layers.DenseLayer
Imports OutputLayer = org.deeplearning4j.nn.conf.layers.OutputLayer
Imports ComputationGraph = org.deeplearning4j.nn.graph.ComputationGraph
Imports MultiLayerNetwork = org.deeplearning4j.nn.multilayer.MultiLayerNetwork
Imports WeightInit = org.deeplearning4j.nn.weights.WeightInit
Imports BaseTrainingListener = org.deeplearning4j.optimize.api.BaseTrainingListener
Imports TrainingListener = org.deeplearning4j.optimize.api.TrainingListener
Imports ThresholdAlgorithm = org.deeplearning4j.optimize.solvers.accumulation.encoding.ThresholdAlgorithm
Imports AdaptiveThresholdAlgorithm = org.deeplearning4j.optimize.solvers.accumulation.encoding.threshold.AdaptiveThresholdAlgorithm
Imports FixedThresholdAlgorithm = org.deeplearning4j.optimize.solvers.accumulation.encoding.threshold.FixedThresholdAlgorithm
Imports RDDTrainingApproach = org.deeplearning4j.spark.api.RDDTrainingApproach
Imports org.deeplearning4j.spark.api
Imports SparkComputationGraph = org.deeplearning4j.spark.impl.graph.SparkComputationGraph
Imports SparkDl4jMultiLayer = org.deeplearning4j.spark.impl.multilayer.SparkDl4jMultiLayer
Imports SharedTrainingMaster = org.deeplearning4j.spark.parameterserver.training.SharedTrainingMaster
Imports Disabled = org.junit.jupiter.api.Disabled
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports TempDir = org.junit.jupiter.api.io.TempDir
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports Activation = org.nd4j.linalg.activations.Activation
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports DataSet = org.nd4j.linalg.dataset.DataSet
Imports MultiDataSet = org.nd4j.linalg.dataset.api.MultiDataSet
Imports DataSetIterator = org.nd4j.linalg.dataset.api.iterator.DataSetIterator
Imports NDArrayIndex = org.nd4j.linalg.indexing.NDArrayIndex
Imports AMSGrad = org.nd4j.linalg.learning.config.AMSGrad
Imports LossFunctions = org.nd4j.linalg.lossfunctions.LossFunctions
Imports Transforms = org.nd4j.linalg.ops.transforms.Transforms
Imports VoidConfiguration = org.nd4j.parameterserver.distributed.conf.VoidConfiguration
Imports MeshBuildMode = org.nd4j.parameterserver.distributed.v2.enums.MeshBuildMode
Imports org.junit.jupiter.api.Assertions
Imports BaseSparkTest = org.deeplearning4j.spark.parameterserver.BaseSparkTest

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

Namespace org.deeplearning4j.spark.parameterserver.train


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j @Tag(TagNames.FILE_IO) @Tag(TagNames.SPARK) @Tag(TagNames.DIST_SYSTEMS) @NativeTag @Tag(TagNames.LONG_TEST) @Tag(TagNames.LARGE_RESOURCES) public class GradientSharingTrainingTest extends org.deeplearning4j.spark.parameterserver.BaseSparkTest
	<Serializable>
	Public Class GradientSharingTrainingTest
		Inherits BaseSparkTest

		Public Overrides ReadOnly Property TimeoutMilliseconds As Long
			Get
				Return 180000L
			End Get
		End Property

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @Disabled public void trainSanityCheck(@TempDir Path testDir) throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub trainSanityCheck(ByVal testDir As Path)

			For Each mds As Boolean In New Boolean(){False, True}
				Dim last As INDArray = Nothing

				Dim lastDup As INDArray = Nothing
				For Each s As String In New String(){"paths", "direSparkSequenceVectorsTestct", "export"}
					Console.WriteLine("--------------------------------------------------------------------------------------------------------------")
					log.info("Starting: {} - {}", s, (If(mds, "MultiDataSet", "DataSet")))
					Dim isPaths As Boolean = "paths".Equals(s)

					Dim rddTrainingApproach As RDDTrainingApproach
					Select Case s
						Case "direct"
							rddTrainingApproach = RDDTrainingApproach.Direct
						Case "export"
							rddTrainingApproach = RDDTrainingApproach.Export
						Case "paths"
							rddTrainingApproach = RDDTrainingApproach.Direct 'Actualy not used for fitPaths
						Case Else
							Throw New Exception()
					End Select

					Dim temp As File = testDir.toFile()


					'TODO this probably won't work everywhere...
					Dim controller As String = Inet4Address.getLocalHost().getHostAddress()
					Dim networkMask As String = controller.Substring(0, controller.LastIndexOf("."c)) & ".0" & "/16"

					Dim voidConfiguration As VoidConfiguration = VoidConfiguration.builder().unicastPort(40123).networkMask(networkMask).controllerAddress(controller).meshBuildMode(MeshBuildMode.PLAIN).build()
					Dim tm As TrainingMaster = (New SharedTrainingMaster.Builder(voidConfiguration, 2, New AdaptiveThresholdAlgorithm(1e-3), 16)).rngSeed(12345).collectTrainingStats(False).batchSizePerWorker(16).workersPerNode(2).rddTrainingApproach(rddTrainingApproach).exportDirectory("file:///" & temp.getAbsolutePath().replaceAll("\\", "/")).build()


					Dim conf As ComputationGraphConfiguration = (New NeuralNetConfiguration.Builder()).seed(12345).updater(New AMSGrad(0.1)).graphBuilder().addInputs("in").layer("out", (New OutputLayer.Builder()).nIn(784).nOut(10).activation(Activation.SOFTMAX).lossFunction(LossFunctions.LossFunction.MCXENT).build(), "in").setOutputs("out").build()


					Dim sparkNet As New SparkComputationGraph(sc, conf, tm)
					sparkNet.CollectTrainingStats = tm.getIsCollectTrainingStats()

	'                System.out.println(Arrays.toString(sparkNet.getNetwork().params().get(NDArrayIndex.point(0), NDArrayIndex.interval(0, 256)).dup().data().asFloat()));
					Dim f As New File(testDir.toFile(),"test-dir-1")
					f.mkdirs()
					Dim iter As DataSetIterator = New MnistDataSetIterator(16, True, 12345)
					Dim count As Integer = 0
					Dim paths As IList(Of String) = New List(Of String)()
					Dim ds As IList(Of DataSet) = New List(Of DataSet)()
'JAVA TO VB CONVERTER TODO TASK: The following line contains an assignment within expression that was not extracted by Java to VB Converter:
'ORIGINAL LINE: while (iter.hasNext() && count++ < 8)
					Do While iter.MoveNext() AndAlso count++ < 8
						Dim d As DataSet = iter.Current
						If isPaths Then
							Dim [out] As New File(f, count & ".bin")
							If mds Then
								d.toMultiDataSet().save([out])
							Else
								d.save([out])
							End If
							Dim path As String = "file:///" & [out].getAbsolutePath().replaceAll("\\", "/")
							paths.Add(path)
						End If
						ds.Add(d)
					Loop

					Dim numIter As Integer = 1
					Dim acc(numIter) As Double
					For i As Integer = 0 To numIter - 1
						'Check accuracy before:
						Dim testIter As DataSetIterator = New EarlyTerminationDataSetIterator(New MnistDataSetIterator(32, False, 12345), 10)
						Dim eBefore As Evaluation = sparkNet.Network.evaluate(testIter)

						Dim paramsBefore As INDArray = sparkNet.Network.params().dup()
						Dim after As ComputationGraph
						If mds Then
							'Fitting from MultiDataSet
							Dim mdsList As IList(Of MultiDataSet) = New List(Of MultiDataSet)()
							For Each d As DataSet In ds
								mdsList.Add(d.toMultiDataSet())
							Next d
							Select Case s
								Case "direct", "export"
									Dim dsRDD As JavaRDD(Of MultiDataSet) = sc.parallelize(mdsList)
									after = sparkNet.fitMultiDataSet(dsRDD)
								Case "paths"
									Dim pathRdd As JavaRDD(Of String) = sc.parallelize(paths)
									after = sparkNet.fitPathsMultiDataSet(pathRdd)
								Case Else
									Throw New Exception()
							End Select
						Else
							'Fitting from DataSet
							Select Case s
								Case "direct", "export"
									Dim dsRDD As JavaRDD(Of DataSet) = sc.parallelize(ds)
									after = sparkNet.fit(dsRDD)
								Case "paths"
									Dim pathRdd As JavaRDD(Of String) = sc.parallelize(paths)
									after = sparkNet.fitPaths(pathRdd)
								Case Else
									Throw New Exception()
							End Select
						End If

						Dim paramsAfter As INDArray = after.params()
	'                    System.out.println(Arrays.toString(paramsBefore.get(NDArrayIndex.point(0), NDArrayIndex.interval(0, 256)).dup().data().asFloat()));
	'                    System.out.println(Arrays.toString(paramsAfter.get(NDArrayIndex.point(0), NDArrayIndex.interval(0, 256)).dup().data().asFloat()));
	'                    System.out.println(Arrays.toString(
	'                            Transforms.abs(paramsAfter.sub(paramsBefore)).get(NDArrayIndex.point(0), NDArrayIndex.interval(0, 256)).dup().data().asFloat()));
						assertNotEquals(paramsBefore, paramsAfter)


						testIter = New EarlyTerminationDataSetIterator(New MnistDataSetIterator(32, False, 12345), 10)
						Dim eAfter As Evaluation = after.evaluate(testIter)

						Dim accAfter As Double = eAfter.accuracy()
						Dim accBefore As Double = eBefore.accuracy()
						assertTrue(accAfter >= accBefore + 0.005, "after: " & accAfter & ", before=" & accBefore)

						If i = 0 Then
							acc(0) = eBefore.accuracy()
						End If
						acc(i + 1) = eAfter.accuracy()
					Next i
					log.info("Accuracies: {}", java.util.Arrays.toString(acc))
					last = sparkNet.Network.params()
					lastDup = last.dup()
				Next s
			Next mds
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @Disabled public void differentNetsTrainingTest(@TempDir Path testDir) throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub differentNetsTrainingTest(ByVal testDir As Path)
			Dim batch As Integer = 3

			Dim temp As File = testDir.toFile()
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			Dim ds As DataSet = (New IrisDataSetIterator(150, 150)).next()
			Dim list As IList(Of DataSet) = ds.asList()
			Collections.shuffle(list, New Random(12345))
			Dim pos As Integer = 0
			Dim dsCount As Integer = 0
			Do While pos < list.Count
				Dim l2 As IList(Of DataSet) = New List(Of DataSet)()
				Dim i As Integer = 0
				Do While i < 3 AndAlso pos < list.Count
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: l2.add(list.get(pos++));
					l2.Add(list(pos))
						pos += 1
					i += 1
				Loop
				Dim d As DataSet = DataSet.merge(l2)
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: java.io.File f = new java.io.File(temp, dsCount++ + ".bin");
				Dim f As New File(temp, dsCount & ".bin")
					dsCount += 1
				d.save(f)
			Loop

			Dim last As INDArray = Nothing
			Dim lastDup As INDArray = Nothing
			For i As Integer = 0 To 1
				Console.WriteLine("||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||")
				log.info("Starting: {}", i)

				Dim conf As MultiLayerConfiguration
				If i = 0 Then
					conf = (New NeuralNetConfiguration.Builder()).weightInit(WeightInit.XAVIER).seed(12345).list().layer((New OutputLayer.Builder()).nIn(4).nOut(3).activation(Activation.SOFTMAX).lossFunction(LossFunctions.LossFunction.MCXENT).build()).build()
				Else
					conf = (New NeuralNetConfiguration.Builder()).weightInit(WeightInit.XAVIER).seed(12345).list().layer((New DenseLayer.Builder()).nIn(4).nOut(4).activation(Activation.TANH).build()).layer((New OutputLayer.Builder()).nIn(4).nOut(3).activation(Activation.SOFTMAX).lossFunction(LossFunctions.LossFunction.MCXENT).build()).build()
				End If
				Dim net As New MultiLayerNetwork(conf)
				net.init()


				'TODO this probably won't work everywhere...
				Dim controller As String = Inet4Address.getLocalHost().getHostAddress()
				Dim networkMask As String = controller.Substring(0, controller.LastIndexOf("."c)) & ".0" & "/16"

				Dim voidConfiguration As VoidConfiguration = VoidConfiguration.builder().unicastPort(40123).networkMask(networkMask).controllerAddress(controller).build()
				Dim tm As TrainingMaster = (New SharedTrainingMaster.Builder(voidConfiguration, 2, New FixedThresholdAlgorithm(1e-4), batch)).rngSeed(12345).collectTrainingStats(False).batchSizePerWorker(batch).workersPerNode(2).build()


				Dim sparkNet As New SparkDl4jMultiLayer(sc, net, tm)

				'System.out.println(Arrays.toString(sparkNet.getNetwork().params().get(NDArrayIndex.point(0), NDArrayIndex.interval(0, 256)).dup().data().asFloat()));

				Dim fitPath As String = "file:///" & temp.getAbsolutePath().replaceAll("\\", "/")
				Dim paramsBefore As INDArray = net.params().dup()
				For j As Integer = 0 To 2
					sparkNet.fit(fitPath)
				Next j

				Dim paramsAfter As INDArray = net.params()
				assertNotEquals(paramsBefore, paramsAfter)

				'Also check we don't have any issues
				If i = 0 Then
					last = sparkNet.Network.params()
					lastDup = last.dup()
				Else
					assertEquals(lastDup, last)
				End If
			Next i
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testEpochUpdating(@TempDir Path testDir) throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testEpochUpdating(ByVal testDir As Path)
			'Ensure that epoch counter is incremented properly on the workers

			Dim temp As File = testDir.resolve("new-dir-" & System.Guid.randomUUID().ToString()).toFile()
			temp.mkdirs()

			'TODO this probably won't work everywhere...
			Dim controller As String = Inet4Address.getLocalHost().getHostAddress()
			Dim networkMask As String = controller.Substring(0, controller.LastIndexOf("."c)) & ".0" & "/16"

			Dim voidConfiguration As VoidConfiguration = VoidConfiguration.builder().unicastPort(40123).networkMask(networkMask).controllerAddress(controller).meshBuildMode(MeshBuildMode.PLAIN).build()
			Dim tm As SharedTrainingMaster = (New SharedTrainingMaster.Builder(voidConfiguration, 2, New AdaptiveThresholdAlgorithm(1e-3), 16)).rngSeed(12345).collectTrainingStats(False).batchSizePerWorker(16).workersPerNode(2).exportDirectory("file:///" & temp.getAbsolutePath().replaceAll("\\", "/")).build()


			Dim conf As ComputationGraphConfiguration = (New NeuralNetConfiguration.Builder()).seed(12345).updater(New AMSGrad(0.001)).graphBuilder().addInputs("in").layer("out", (New OutputLayer.Builder()).nIn(784).nOut(10).activation(Activation.SOFTMAX).lossFunction(LossFunctions.LossFunction.MCXENT).build(), "in").setOutputs("out").build()


			Dim sparkNet As New SparkComputationGraph(sc, conf, tm)
			sparkNet.setListeners(New TestListener())

			Dim iter As DataSetIterator = New MnistDataSetIterator(16, True, 12345)
			Dim count As Integer = 0
			Dim paths As IList(Of String) = New List(Of String)()
			Dim ds As IList(Of DataSet) = New List(Of DataSet)()
			Dim f As New File(testDir.toFile(),"test-dir-1")
			f.mkdirs()
'JAVA TO VB CONVERTER TODO TASK: The following line contains an assignment within expression that was not extracted by Java to VB Converter:
'ORIGINAL LINE: while (iter.hasNext() && count++ < 8)
			Do While iter.MoveNext() AndAlso count++ < 8
				Dim d As DataSet = iter.Current
				Dim [out] As New File(f, count & ".bin")
				d.save([out])
				Dim path As String = "file:///" & [out].getAbsolutePath().replaceAll("\\", "/")
				paths.Add(path)
				ds.Add(d)
			Loop

			Dim pathRdd As JavaRDD(Of String) = sc.parallelize(paths)
			For i As Integer = 0 To 2
				Dim ta As ThresholdAlgorithm = tm.getThresholdAlgorithm()
				sparkNet.fitPaths(pathRdd)
				'Check also that threshold algorithm was updated/averaged
				Dim taAfter As ThresholdAlgorithm = tm.getThresholdAlgorithm()
				assertTrue(ta IsNot taAfter, "Threshold algorithm should have been updated with different instance after averaging")
				Dim ataAfter As AdaptiveThresholdAlgorithm = DirectCast(taAfter, AdaptiveThresholdAlgorithm)
				assertFalse(Double.IsNaN(ataAfter.getLastSparsity()))
				assertFalse(Double.IsNaN(ataAfter.getLastThreshold()))
			Next i

			Dim expectedEpochs As ISet(Of Integer) = New HashSet(Of Integer)(java.util.Arrays.asList(0, 1, 2))
			assertEquals(expectedEpochs, TestListener.epochs)
		End Sub

		<Serializable>
		Private Class TestListener
			Inherits BaseTrainingListener

			Friend Shared ReadOnly iterations As ISet(Of Integer) = Collections.newSetFromMap(New ConcurrentDictionary(Of Integer)())
			Friend Shared ReadOnly epochs As ISet(Of Integer) = Collections.newSetFromMap(New ConcurrentDictionary(Of Integer)())
			Public Overrides Sub iterationDone(ByVal model As Model, ByVal iteration As Integer, ByVal epoch As Integer)
				iterations.Add(iteration)
				epochs.Add(epoch)
			End Sub
		End Class
	End Class

End Namespace