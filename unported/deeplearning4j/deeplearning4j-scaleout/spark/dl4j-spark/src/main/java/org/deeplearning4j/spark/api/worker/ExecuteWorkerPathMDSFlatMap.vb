Imports System
Imports System.Collections.Generic
Imports FlatMapFunction = org.apache.spark.api.java.function.FlatMapFunction
Imports Broadcast = org.apache.spark.broadcast.Broadcast
Imports SerializableHadoopConfig = org.datavec.spark.util.SerializableHadoopConfig
Imports MultiDataSetLoader = org.deeplearning4j.core.loader.MultiDataSetLoader
Imports TrainingResult = org.deeplearning4j.spark.api.TrainingResult
Imports org.deeplearning4j.spark.api
Imports WorkerConfiguration = org.deeplearning4j.spark.api.WorkerConfiguration
Imports PathSparkMultiDataSetIterator = org.deeplearning4j.spark.iterator.PathSparkMultiDataSetIterator
Imports MultiDataSet = org.nd4j.linalg.dataset.api.MultiDataSet

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

Namespace org.deeplearning4j.spark.api.worker


	Public Class ExecuteWorkerPathMDSFlatMap(Of R As org.deeplearning4j.spark.api.TrainingResult)
		Implements FlatMapFunction(Of IEnumerator(Of String), R)

		Private ReadOnly workerFlatMap As FlatMapFunction(Of IEnumerator(Of MultiDataSet), R)
		Private loader As MultiDataSetLoader
		Private ReadOnly maxDataSetObjects As Integer
		Private ReadOnly hadoopConfig As Broadcast(Of SerializableHadoopConfig)

		Public Sub New(ByVal worker As TrainingWorker(Of R), ByVal loader As MultiDataSetLoader, ByVal hadoopConfig As Broadcast(Of SerializableHadoopConfig))
			Me.workerFlatMap = New ExecuteWorkerMultiDataSetFlatMap(Of IEnumerator(Of MultiDataSet), R)(worker)
			Me.loader = loader
			Me.hadoopConfig = hadoopConfig

			'How many dataset objects of size 'dataSetObjectNumExamples' should we load?
			'Only pass on the required number, not all of them (to avoid async preloading data that won't be used)
			'Most of the time we'll get exactly the number we want, but this isn't guaranteed all the time for all
			' splitting strategies
			Dim conf As WorkerConfiguration = worker.DataConfiguration
			Dim dataSetObjectNumExamples As Integer = conf.getDataSetObjectSizeExamples()
			Dim workerMinibatchSize As Integer = conf.getBatchSizePerWorker()
			Dim maxMinibatches As Integer = (If(conf.getMaxBatchesPerWorker() > 0, conf.getMaxBatchesPerWorker(), Integer.MaxValue))

			If maxMinibatches = Integer.MaxValue Then
				maxDataSetObjects = Integer.MaxValue
			Else
				'Required: total number of examples / examples per dataset object
				maxDataSetObjects = CInt(Math.Truncate(Math.Ceiling(maxMinibatches * workerMinibatchSize / (CDbl(dataSetObjectNumExamples)))))
			End If
		End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public java.util.Iterator<R> call(java.util.Iterator<String> iter) throws Exception
		Public Overrides Function [call](ByVal iter As IEnumerator(Of String)) As IEnumerator(Of R)
			Dim list As IList(Of String) = New List(Of String)()
			Dim count As Integer = 0
'JAVA TO VB CONVERTER TODO TASK: The following line contains an assignment within expression that was not extracted by Java to VB Converter:
'ORIGINAL LINE: while (iter.hasNext() && count++ < maxDataSetObjects)
			Do While iter.MoveNext() AndAlso count++ < maxDataSetObjects
				list.Add(iter.Current)
			Loop

			Return workerFlatMap.call(New PathSparkMultiDataSetIterator(list.GetEnumerator(), loader, hadoopConfig))
		End Function
	End Class

End Namespace