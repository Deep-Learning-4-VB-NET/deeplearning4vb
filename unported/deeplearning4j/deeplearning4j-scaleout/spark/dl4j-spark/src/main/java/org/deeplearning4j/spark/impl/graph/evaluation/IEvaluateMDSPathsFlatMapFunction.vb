Imports System.Collections.Generic
Imports FlatMapFunction = org.apache.spark.api.java.function.FlatMapFunction
Imports Broadcast = org.apache.spark.broadcast.Broadcast
Imports SerializableHadoopConfig = org.datavec.spark.util.SerializableHadoopConfig
Imports DataSetLoader = org.deeplearning4j.core.loader.DataSetLoader
Imports MultiDataSetLoader = org.deeplearning4j.core.loader.MultiDataSetLoader
Imports DataSetLoaderIterator = org.deeplearning4j.datasets.iterator.loader.DataSetLoaderIterator
Imports MultiDataSetLoaderIterator = org.deeplearning4j.datasets.iterator.loader.MultiDataSetLoaderIterator
Imports RemoteFileSourceFactory = org.deeplearning4j.spark.data.loader.RemoteFileSourceFactory
Imports EvaluationRunner = org.deeplearning4j.spark.impl.evaluation.EvaluationRunner
Imports org.nd4j.evaluation
Imports MultiDataSetIteratorAdapter = org.nd4j.linalg.dataset.adapter.MultiDataSetIteratorAdapter
Imports DataSetIterator = org.nd4j.linalg.dataset.api.iterator.DataSetIterator
Imports MultiDataSetIterator = org.nd4j.linalg.dataset.api.iterator.MultiDataSetIterator

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

Namespace org.deeplearning4j.spark.impl.graph.evaluation


	Public Class IEvaluateMDSPathsFlatMapFunction
		Implements FlatMapFunction(Of IEnumerator(Of String), IEvaluation())

		Protected Friend json As Broadcast(Of String)
		Protected Friend params As Broadcast(Of SByte())
		Protected Friend evalNumWorkers As Integer
		Protected Friend evalBatchSize As Integer
		Protected Friend dsLoader As DataSetLoader
		Protected Friend mdsLoader As MultiDataSetLoader
		Protected Friend conf As Broadcast(Of SerializableHadoopConfig)
		Protected Friend evaluations() As IEvaluation

		''' <param name="json"> Network configuration (json format) </param>
		''' <param name="params"> Network parameters </param>
		''' <param name="evalBatchSize"> Max examples per evaluation. Do multiple separate forward passes if data exceeds
		'''                              this. Used to avoid doing too many at once (and hence memory issues) </param>
		''' <param name="evaluations"> Initial evaulation instance (i.e., empty Evaluation or RegressionEvaluation instance) </param>
		Public Sub New(ByVal json As Broadcast(Of String), ByVal params As Broadcast(Of SByte()), ByVal evalNumWorkers As Integer, ByVal evalBatchSize As Integer, ByVal dsLoader As DataSetLoader, ByVal mdsLoader As MultiDataSetLoader, ByVal configuration As Broadcast(Of SerializableHadoopConfig), ByVal evaluations() As IEvaluation)
			Me.json = json
			Me.params = params
			Me.evalNumWorkers = evalNumWorkers
			Me.evalBatchSize = evalBatchSize
			Me.dsLoader = dsLoader
			Me.mdsLoader = mdsLoader
			Me.conf = configuration
			Me.evaluations = evaluations
		End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public java.util.Iterator<org.nd4j.evaluation.IEvaluation[]> call(java.util.Iterator<String> paths) throws Exception
		Public Overrides Function [call](ByVal paths As IEnumerator(Of String)) As IEnumerator(Of IEvaluation())
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			If Not paths.hasNext() Then
				Return Collections.emptyIterator()
			End If

			Dim iter As MultiDataSetIterator
			If dsLoader IsNot Nothing Then
				Dim dsIter As DataSetIterator = New DataSetLoaderIterator(paths, dsLoader, New RemoteFileSourceFactory(conf))
				iter = New MultiDataSetIteratorAdapter(dsIter)
			Else
				iter = New MultiDataSetLoaderIterator(paths, mdsLoader, New RemoteFileSourceFactory(conf))
			End If

			Dim f As Future(Of IEvaluation()) = EvaluationRunner.Instance.execute(evaluations, evalNumWorkers, evalBatchSize, Nothing, iter, True, json, params)
			Dim result() As IEvaluation = f.get()
			If result Is Nothing Then
				Return Collections.emptyIterator()
			Else
				Return Collections.singletonList(result).GetEnumerator()
			End If
		End Function
	End Class

End Namespace