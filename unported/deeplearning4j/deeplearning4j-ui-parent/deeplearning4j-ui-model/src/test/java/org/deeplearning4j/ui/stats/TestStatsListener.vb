Imports System
Imports System.Collections.Generic
Imports Persistable = org.deeplearning4j.core.storage.Persistable
Imports StatsStorage = org.deeplearning4j.core.storage.StatsStorage
Imports IrisDataSetIterator = org.deeplearning4j.datasets.iterator.impl.IrisDataSetIterator
Imports OptimizationAlgorithm = org.deeplearning4j.nn.api.OptimizationAlgorithm
Imports MultiLayerConfiguration = org.deeplearning4j.nn.conf.MultiLayerConfiguration
Imports NeuralNetConfiguration = org.deeplearning4j.nn.conf.NeuralNetConfiguration
Imports OutputLayer = org.deeplearning4j.nn.conf.layers.OutputLayer
Imports MultiLayerNetwork = org.deeplearning4j.nn.multilayer.MultiLayerNetwork
Imports BaseDL4JTest = org.deeplearning4j.BaseDL4JTest
Imports J7StatsListener = org.deeplearning4j.ui.model.stats.J7StatsListener
Imports StatsListener = org.deeplearning4j.ui.model.stats.StatsListener
Imports MapDBStatsStorage = org.deeplearning4j.ui.model.storage.mapdb.MapDBStatsStorage
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports Activation = org.nd4j.linalg.activations.Activation
Imports DataSet = org.nd4j.linalg.dataset.DataSet
Imports LossFunctions = org.nd4j.linalg.lossfunctions.LossFunctions
import static org.junit.jupiter.api.Assertions.assertEquals
import static org.junit.jupiter.api.Assertions.assertNotNull

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

Namespace org.deeplearning4j.ui.stats


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Tag(TagNames.FILE_IO) @Tag(TagNames.UI) @Tag(TagNames.DIST_SYSTEMS) @NativeTag public class TestStatsListener extends org.deeplearning4j.BaseDL4JTest
	Public Class TestStatsListener
		Inherits BaseDL4JTest

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testListenerBasic()
		Public Overridable Sub testListenerBasic()

			For Each useJ7 As Boolean In New Boolean() {False, True}

'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
				Dim ds As DataSet = (New IrisDataSetIterator(150, 150)).next()

				Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).optimizationAlgo(OptimizationAlgorithm.STOCHASTIC_GRADIENT_DESCENT).list().layer(0, (New OutputLayer.Builder(LossFunctions.LossFunction.MCXENT)).activation(Activation.SOFTMAX).nIn(4).nOut(3).build()).build()

				Dim net As New MultiLayerNetwork(conf)
				net.init()

				Dim ss As StatsStorage = New MapDBStatsStorage() 'in-memory

				If useJ7 Then
					net.setListeners(New J7StatsListener(ss, 1))
				Else
					net.setListeners(New StatsListener(ss, 1))
				End If


				For i As Integer = 0 To 2
					net.fit(ds)
				Next i

				Dim sids As IList(Of String) = ss.listSessionIDs()
				assertEquals(1, sids.Count)
				Dim sessionID As String = ss.listSessionIDs()(0)
				assertEquals(1, ss.listTypeIDsForSession(sessionID).Count)
				Dim typeID As String = ss.listTypeIDsForSession(sessionID)(0)
				assertEquals(1, ss.listWorkerIDsForSession(sessionID).Count)
				Dim workerID As String = ss.listWorkerIDsForSession(sessionID)(0)

				Dim staticInfo As Persistable = ss.getStaticInfo(sessionID, typeID, workerID)
				assertNotNull(staticInfo)
				Console.WriteLine(staticInfo)

				Dim updates As IList(Of Persistable) = ss.getAllUpdatesAfter(sessionID, typeID, workerID, 0)
				assertEquals(3, updates.Count)
				For Each p As Persistable In updates
					Console.WriteLine(p)
				Next p

			Next useJ7

		End Sub

	End Class

End Namespace