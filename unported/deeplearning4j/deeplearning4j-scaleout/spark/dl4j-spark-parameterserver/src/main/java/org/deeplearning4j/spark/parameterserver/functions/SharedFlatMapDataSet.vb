Imports System.Collections.Generic
Imports FlatMapFunction = org.apache.spark.api.java.function.FlatMapFunction
Imports TrainingResult = org.deeplearning4j.spark.api.TrainingResult
Imports org.deeplearning4j.spark.api
Imports SharedTrainingWrapper = org.deeplearning4j.spark.parameterserver.pw.SharedTrainingWrapper
Imports SharedTrainingResult = org.deeplearning4j.spark.parameterserver.training.SharedTrainingResult
Imports SharedTrainingWorker = org.deeplearning4j.spark.parameterserver.training.SharedTrainingWorker
Imports DataSet = org.nd4j.linalg.dataset.DataSet

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


	Public Class SharedFlatMapDataSet(Of R As org.deeplearning4j.spark.api.TrainingResult)
		Implements FlatMapFunction(Of IEnumerator(Of DataSet), R)

		Private ReadOnly worker As SharedTrainingWorker

		Public Sub New(ByVal worker As TrainingWorker(Of R))
			' we're not going to have anything but Shared classes here ever
			Me.worker = CType(worker, SharedTrainingWorker)
		End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public java.util.Iterator<R> call(java.util.Iterator<org.nd4j.linalg.dataset.DataSet> dataSetIterator) throws Exception
		Public Overrides Function [call](ByVal dataSetIterator As IEnumerator(Of DataSet)) As IEnumerator(Of R)
			'Under some limited circumstances, we might have an empty partition. In this case, we should return immediately
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			If Not dataSetIterator.hasNext() Then
				Return Collections.emptyIterator()
			End If

	'        
	'            That's the place where we do our stuff. Here's the plan:
	'            1) we pass given iterator to VirtualDataSetIterator, which acts as holder for them
	'            2) Virtual iterator will provide load balancing between available devices
	'            3) we'll lock out here
	'         

			' iterator should be silently attached to VirtualDataSetIterator, and used appropriately
			SharedTrainingWrapper.getInstance(worker.InstanceId).attachDS(dataSetIterator)

			' first callee will become master, others will obey and die
			' all threads in this executor will be blocked here until training finished
			Dim result As SharedTrainingResult = SharedTrainingWrapper.getInstance(worker.InstanceId).run(worker)

			Return Collections.singletonList(CType(result, R)).GetEnumerator()
		End Function
	End Class

End Namespace