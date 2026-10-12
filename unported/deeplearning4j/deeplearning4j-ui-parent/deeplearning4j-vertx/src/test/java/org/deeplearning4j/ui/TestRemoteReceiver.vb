Imports System.Collections.Generic
Imports System.Threading
Imports BaseDL4JTest = org.deeplearning4j.BaseDL4JTest
Imports Persistable = org.deeplearning4j.core.storage.Persistable
Imports StorageMetaData = org.deeplearning4j.core.storage.StorageMetaData
Imports CollectionStatsStorageRouter = org.deeplearning4j.core.storage.impl.CollectionStatsStorageRouter
Imports RemoteUIStatsStorageRouter = org.deeplearning4j.core.storage.impl.RemoteUIStatsStorageRouter
Imports IrisDataSetIterator = org.deeplearning4j.datasets.iterator.impl.IrisDataSetIterator
Imports OptimizationAlgorithm = org.deeplearning4j.nn.api.OptimizationAlgorithm
Imports MultiLayerConfiguration = org.deeplearning4j.nn.conf.MultiLayerConfiguration
Imports NeuralNetConfiguration = org.deeplearning4j.nn.conf.NeuralNetConfiguration
Imports DenseLayer = org.deeplearning4j.nn.conf.layers.DenseLayer
Imports OutputLayer = org.deeplearning4j.nn.conf.layers.OutputLayer
Imports MultiLayerNetwork = org.deeplearning4j.nn.multilayer.MultiLayerNetwork
Imports ScoreIterationListener = org.deeplearning4j.optimize.listeners.ScoreIterationListener
Imports UIServer = org.deeplearning4j.ui.api.UIServer
Imports StatsListener = org.deeplearning4j.ui.model.stats.StatsListener
Imports SbeStatsInitializationReport = org.deeplearning4j.ui.model.stats.impl.SbeStatsInitializationReport
Imports SbeStatsReport = org.deeplearning4j.ui.model.stats.impl.SbeStatsReport
Imports SbeStorageMetaData = org.deeplearning4j.ui.model.storage.impl.SbeStorageMetaData
Imports Disabled = org.junit.jupiter.api.Disabled
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports Activation = org.nd4j.linalg.activations.Activation
Imports DataSetIterator = org.nd4j.linalg.dataset.api.iterator.DataSetIterator
Imports LossFunctions = org.nd4j.linalg.lossfunctions.LossFunctions
import static org.junit.jupiter.api.Assertions.assertEquals

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

Namespace org.deeplearning4j.ui


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Disabled @Tag(TagNames.FILE_IO) @Tag(TagNames.UI) @Tag(TagNames.DIST_SYSTEMS) @NativeTag public class TestRemoteReceiver extends org.deeplearning4j.BaseDL4JTest
	Public Class TestRemoteReceiver
		Inherits BaseDL4JTest

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @Disabled public void testRemoteBasic() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testRemoteBasic()

			Dim updates As IList(Of Persistable) = New List(Of Persistable)()
			Dim staticInfo As IList(Of Persistable) = New List(Of Persistable)()
			Dim metaData As IList(Of StorageMetaData) = New List(Of StorageMetaData)()
			Dim collectionRouter As New CollectionStatsStorageRouter(metaData, staticInfo, updates)


			Dim s As UIServer = UIServer.getInstance()
			Thread.Sleep(1000)
			s.enableRemoteListener(collectionRouter, False)


			Using remoteRouter As New org.deeplearning4j.core.storage.impl.RemoteUIStatsStorageRouter("http://localhost:9000") 'Use closeable to ensure async thread is shut down

				Dim update1 As New SbeStatsReport()
				update1.setMemoryUsePresent(True)
				update1.setDeviceCurrentBytes(New Long(){1, 2})
				update1.setDeviceMaxBytes(New Long(){100, 200})
				update1.reportIterationCount(10)
				update1.reportIDs("sid", "tid", "wid", 123456)
				update1.reportPerformance(10, 20, 30, 40, 50)

				Dim update2 As New SbeStatsReport()
				update2.setMemoryUsePresent(True)
				update2.setDeviceCurrentBytes(New Long(){3, 4})
				update2.setDeviceMaxBytes(New Long(){300, 400})
				update2.reportIterationCount(20)
				update2.reportIDs("sid2", "tid2", "wid2", 123456)
				update2.reportPerformance(11, 21, 31, 40, 50)

				Dim smd1 As StorageMetaData = New SbeStorageMetaData(123, "sid", "typeid", "wid", "initTypeClass", "updaterTypeClass")
				Dim smd2 As StorageMetaData = New SbeStorageMetaData(456, "sid2", "typeid2", "wid2", "initTypeClass2", "updaterTypeClass2")

				Dim init1 As New SbeStatsInitializationReport()
				init1.reportIDs("sid", "wid", "tid", 3145253452L)
				init1.reportHardwareInfo(1, 2, 3, 4, Nothing, Nothing, "2344253")
				init1.setHwDeviceTotalMemory(New Long(){1, 2})
				init1.setHwDeviceDescription(New String(){"d1", "d2"})
				init1.setHasHardwareInfo(True)

				remoteRouter.putUpdate(update1)
				Thread.Sleep(100)
				remoteRouter.putStorageMetaData(smd1)
				Thread.Sleep(100)
				remoteRouter.putStaticInfo(init1)
				Thread.Sleep(100)
				remoteRouter.putUpdate(update2)
				Thread.Sleep(100)
				remoteRouter.putStorageMetaData(smd2)


				Thread.Sleep(2000)

				assertEquals(2, metaData.Count)
				assertEquals(2, updates.Count)
				assertEquals(1, staticInfo.Count)

				assertEquals(Arrays.asList(update1, update2), updates)
				assertEquals(Arrays.asList(smd1, smd2), metaData)
				assertEquals(Collections.singletonList(init1), staticInfo)
			End Using
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @Disabled public void testRemoteFull() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testRemoteFull()
			'Use this in conjunction with startRemoteUI()

			Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).optimizationAlgo(OptimizationAlgorithm.STOCHASTIC_GRADIENT_DESCENT).list().layer(0, (New DenseLayer.Builder()).activation(Activation.TANH).nIn(4).nOut(4).build()).layer(1, (New OutputLayer.Builder()).lossFunction(LossFunctions.LossFunction.MCXENT).activation(Activation.SOFTMAX).nIn(4).nOut(3).build()).build()

			Dim net As New MultiLayerNetwork(conf)
			net.init()
			Using ssr As New org.deeplearning4j.core.storage.impl.RemoteUIStatsStorageRouter("http://localhost:9000")
				net.setListeners(New StatsListener(ssr), New ScoreIterationListener(1))

				Dim iter As DataSetIterator = New IrisDataSetIterator(150, 150)

				For i As Integer = 0 To 499
					net.fit(iter)
					'            Thread.sleep(100);
					Thread.Sleep(100)
				Next i
			End Using
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @Disabled public void startRemoteUI() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub startRemoteUI()

			Dim s As UIServer = UIServer.getInstance()
			s.enableRemoteListener()

			Thread.Sleep(1000000)
		End Sub

	End Class

End Namespace