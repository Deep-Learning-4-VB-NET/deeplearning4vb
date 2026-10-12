Imports System.Collections.Generic
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports FlatMapFunction = org.apache.spark.api.java.function.FlatMapFunction
Imports Broadcast = org.apache.spark.broadcast.Broadcast
Imports EvaluationRunner = org.deeplearning4j.spark.impl.evaluation.EvaluationRunner
Imports org.nd4j.evaluation
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

Namespace org.deeplearning4j.spark.impl.graph.evaluation


	Public Class IEvaluateMDSFlatMapFunction(Of T As org.nd4j.evaluation.IEvaluation)
		Implements FlatMapFunction(Of IEnumerator(Of MultiDataSet), T())

		Protected Friend json As Broadcast(Of String)
		Protected Friend params As Broadcast(Of SByte())
		Protected Friend evalNumWorkers As Integer
		Protected Friend evalBatchSize As Integer
		Protected Friend evaluations() As T

		''' <param name="json"> Network configuration (json format) </param>
		''' <param name="params"> Network parameters </param>
		''' <param name="evalBatchSize"> Max examples per evaluation. Do multiple separate forward passes if data exceeds
		'''                              this. Used to avoid doing too many at once (and hence memory issues) </param>
		''' <param name="evaluations"> Initial evaulation instance (i.e., empty Evaluation or RegressionEvaluation instance) </param>
		Public Sub New(ByVal json As Broadcast(Of String), ByVal params As Broadcast(Of SByte()), ByVal evalNumWorkers As Integer, ByVal evalBatchSize As Integer, ByVal evaluations() As T)
			Me.json = json
			Me.params = params
			Me.evalNumWorkers = evalNumWorkers
			Me.evalBatchSize = evalBatchSize
			Me.evaluations = evaluations
		End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public java.util.Iterator<T[]> call(java.util.Iterator<org.nd4j.linalg.dataset.api.MultiDataSet> dataSetIterator) throws Exception
		Public Overrides Function [call](ByVal dataSetIterator As IEnumerator(Of MultiDataSet)) As IEnumerator(Of T())
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			If Not dataSetIterator.hasNext() Then
				Return Collections.emptyIterator()
			End If

'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			If Not dataSetIterator.hasNext() Then
				Return Collections.emptyIterator()
			End If

			Dim f As Future(Of IEvaluation()) = EvaluationRunner.Instance.execute(evaluations, evalNumWorkers, evalBatchSize, Nothing, dataSetIterator, True, json, params)

			Dim result() As IEvaluation = f.get()
			If result Is Nothing Then
				Return Collections.emptyIterator()
			Else
				Return Collections.singletonList(CType(result, T())).GetEnumerator()
			End If
		End Function
	End Class

End Namespace