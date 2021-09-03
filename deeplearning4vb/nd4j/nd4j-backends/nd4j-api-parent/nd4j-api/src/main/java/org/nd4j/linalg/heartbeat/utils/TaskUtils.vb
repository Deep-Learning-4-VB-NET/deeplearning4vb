Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports DataSet = org.nd4j.linalg.dataset.DataSet
Imports DataSetIterator = org.nd4j.linalg.dataset.api.iterator.DataSetIterator
Imports Task = org.nd4j.linalg.heartbeat.reports.Task

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

Namespace org.nd4j.linalg.heartbeat.utils

	Public Class TaskUtils
		Private Sub New()
		End Sub

		Public Shared Function buildTask(ByVal array() As INDArray, ByVal labels() As INDArray) As Task
			Dim task As New Task()

			Return task
		End Function

		Public Shared Function buildTask(ByVal array As INDArray, ByVal labels As INDArray) As Task
			Return New Task()
		End Function

		Public Shared Function buildTask(ByVal array As INDArray) As Task
			Return New Task()
		End Function

		Public Shared Function buildTask(ByVal dataSet As DataSet) As Task
			Return New Task()
		End Function

		Public Shared Function buildTask(ByVal dataSet As org.nd4j.linalg.dataset.api.DataSet) As Task
			Return New Task()
		End Function

		Public Shared Function buildTask(ByVal dataSetIterator As DataSetIterator) As Task
			Return New Task()
		End Function
	End Class

End Namespace