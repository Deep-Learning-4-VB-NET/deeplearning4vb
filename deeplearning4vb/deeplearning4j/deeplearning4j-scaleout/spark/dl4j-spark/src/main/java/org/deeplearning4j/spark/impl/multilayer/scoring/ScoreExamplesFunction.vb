Imports System.Collections.Generic
Imports DoubleFlatMapFunction = org.apache.spark.api.java.function.DoubleFlatMapFunction
Imports FlatMapFunction = org.apache.spark.api.java.function.FlatMapFunction
Imports Broadcast = org.apache.spark.broadcast.Broadcast
Imports MultiLayerConfiguration = org.deeplearning4j.nn.conf.MultiLayerConfiguration
Imports MultiLayerNetwork = org.deeplearning4j.nn.multilayer.MultiLayerNetwork
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports DataSet = org.nd4j.linalg.dataset.DataSet
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports Logger = org.slf4j.Logger
Imports LoggerFactory = org.slf4j.LoggerFactory

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

Namespace org.deeplearning4j.spark.impl.multilayer.scoring


	Public Class ScoreExamplesFunction
		Implements DoubleFlatMapFunction(Of IEnumerator(Of DataSet))

		Protected Friend Shared log As Logger = LoggerFactory.getLogger(GetType(ScoreExamplesFunction))

		Private ReadOnly params As Broadcast(Of INDArray)
		Private ReadOnly jsonConfig As Broadcast(Of String)
		Private ReadOnly addRegularization As Boolean
		Private ReadOnly batchSize As Integer

		Public Sub New(ByVal params As Broadcast(Of INDArray), ByVal jsonConfig As Broadcast(Of String), ByVal addRegularizationTerms As Boolean, ByVal batchSize As Integer)
			Me.params = params
			Me.jsonConfig = jsonConfig
			Me.addRegularization = addRegularizationTerms
			Me.batchSize = batchSize
		End Sub


'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public java.util.Iterator<Double> call(java.util.Iterator<org.nd4j.linalg.dataset.DataSet> iterator) throws Exception
		Public Overrides Function [call](ByVal iterator As IEnumerator(Of DataSet)) As IEnumerator(Of Double)
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			If Not iterator.hasNext() Then
				Return Collections.emptyIterator()
			End If

			Dim network As New MultiLayerNetwork(MultiLayerConfiguration.fromJson(jsonConfig.getValue()))
			network.init()
			Dim val As INDArray = params.value().unsafeDuplication()
			If val.length() <> network.numParams(False) Then
				Throw New System.InvalidOperationException("Network did not have same number of parameters as the broadcast set parameters")
			End If
			network.Parameters = val

			Dim ret As IList(Of Double) = New List(Of Double)()

			Dim collect As IList(Of DataSet) = New List(Of DataSet)(batchSize)
			Dim totalCount As Integer = 0
			Do While iterator.MoveNext()
				collect.Clear()
				Dim nExamples As Integer = 0
				Do While iterator.MoveNext() AndAlso nExamples < batchSize
					Dim ds As DataSet = iterator.Current
					Dim n As Integer = ds.numExamples()
					collect.Add(ds)
					nExamples += n
				Loop
				totalCount += nExamples

				Dim data As DataSet = DataSet.merge(collect)


				Dim scores As INDArray = network.scoreExamples(data, addRegularization)
				Dim doubleScores() As Double = scores.data().asDouble()

				For Each doubleScore As Double In doubleScores
					ret.Add(doubleScore)
				Next doubleScore
			Loop

			Nd4j.Executioner.commit()

			If log.isDebugEnabled() Then
				log.debug("Scored {} examples ", totalCount)
			End If

			Return ret.GetEnumerator()
		End Function
	End Class

End Namespace