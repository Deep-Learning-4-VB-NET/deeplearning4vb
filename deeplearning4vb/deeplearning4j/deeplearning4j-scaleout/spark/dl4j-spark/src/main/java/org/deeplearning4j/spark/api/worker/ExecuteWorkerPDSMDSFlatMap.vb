﻿Imports System
Imports System.Collections.Generic
Imports FlatMapFunction = org.apache.spark.api.java.function.FlatMapFunction
Imports PortableDataStream = org.apache.spark.input.PortableDataStream
Imports TrainingResult = org.deeplearning4j.spark.api.TrainingResult
Imports org.deeplearning4j.spark.api
Imports PortableDataStreamMultiDataSetIterator = org.deeplearning4j.spark.iterator.PortableDataStreamMultiDataSetIterator
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


	<Obsolete>
	Public Class ExecuteWorkerPDSMDSFlatMap(Of R As org.deeplearning4j.spark.api.TrainingResult)
		Implements FlatMapFunction(Of IEnumerator(Of PortableDataStream), R)

		Private ReadOnly workerFlatMap As FlatMapFunction(Of IEnumerator(Of MultiDataSet), R)

		Public Sub New(ByVal worker As TrainingWorker(Of R))
			Me.workerFlatMap = New ExecuteWorkerMultiDataSetFlatMap(Of IEnumerator(Of MultiDataSet), R)(worker)
		End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public java.util.Iterator<R> call(java.util.Iterator<org.apache.spark.input.PortableDataStream> iter) throws Exception
		Public Overrides Function [call](ByVal iter As IEnumerator(Of PortableDataStream)) As IEnumerator(Of R)
			Return workerFlatMap.call(New PortableDataStreamMultiDataSetIterator(iter))
		End Function
	End Class

End Namespace