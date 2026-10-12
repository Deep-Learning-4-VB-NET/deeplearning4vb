Imports System.Collections.Generic
Imports System.IO
Imports LineIterator = org.apache.commons.io.LineIterator
Imports FlatMapFunction = org.apache.spark.api.java.function.FlatMapFunction
Imports Broadcast = org.apache.spark.broadcast.Broadcast
Imports SerializableHadoopConfig = org.datavec.spark.util.SerializableHadoopConfig
Imports MultiDataSetLoader = org.deeplearning4j.core.loader.MultiDataSetLoader
Imports TrainingResult = org.deeplearning4j.spark.api.TrainingResult
Imports org.deeplearning4j.spark.api
Imports PathSparkMultiDataSetIterator = org.deeplearning4j.spark.iterator.PathSparkMultiDataSetIterator
Imports SharedTrainingWrapper = org.deeplearning4j.spark.parameterserver.pw.SharedTrainingWrapper
Imports SharedTrainingResult = org.deeplearning4j.spark.parameterserver.training.SharedTrainingResult
Imports SharedTrainingWorker = org.deeplearning4j.spark.parameterserver.training.SharedTrainingWorker

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

Namespace org.deeplearning4j.spark.parameterserver.functions


	Public Class SharedFlatMapPathsMDS(Of R As org.deeplearning4j.spark.api.TrainingResult)
		Implements FlatMapFunction(Of IEnumerator(Of String), R)

		Protected Friend ReadOnly worker As SharedTrainingWorker
		Protected Friend ReadOnly loader As MultiDataSetLoader
		Protected Friend ReadOnly hadoopConfig As Broadcast(Of SerializableHadoopConfig)

		Public Sub New(ByVal worker As TrainingWorker(Of R), ByVal loader As MultiDataSetLoader, ByVal hadoopConfig As Broadcast(Of SerializableHadoopConfig))
			' we're not going to have anything but Shared classes here ever
			Me.worker = CType(worker, SharedTrainingWorker)
			Me.loader = loader
			Me.hadoopConfig = hadoopConfig
		End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public java.util.Iterator<R> call(java.util.Iterator<String> dataSetIterator) throws Exception
		Public Overrides Function [call](ByVal dataSetIterator As IEnumerator(Of String)) As IEnumerator(Of R)
			'Under some limited circumstances, we might have an empty partition. In this case, we should return immediately
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			If Not dataSetIterator.hasNext() Then
				Return Collections.emptyIterator()
			End If
			' here we'll be converting out Strings coming out of iterator to DataSets
			' PathSparkDataSetIterator does that for us
			'For better fault tolerance, we'll pull all paths to a local file. This way, if the Iterator<String> is backed
			' by a remote source that later goes down, we won't fail (as long as the source is still available)
			Dim f As File = SharedFlatMapPaths.toTempFile(dataSetIterator)

			Dim lineIter As New LineIterator(New StreamReader(f)) 'Buffered reader added automatically
			Try
				' iterator should be silently attached to VirtualDataSetIterator, and used appropriately
				SharedTrainingWrapper.getInstance(worker.InstanceId).attachMDS(New PathSparkMultiDataSetIterator(lineIter, loader, hadoopConfig))

				' first callee will become master, others will obey and die
				Dim result As SharedTrainingResult = SharedTrainingWrapper.getInstance(worker.InstanceId).run(worker)

				Return Collections.singletonList(CType(result, R)).GetEnumerator()
			Finally
				lineIter.close()
				f.delete()
			End Try
		End Function
	End Class

End Namespace