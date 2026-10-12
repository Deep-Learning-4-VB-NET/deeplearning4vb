Imports System
Imports System.Collections.Generic
Imports NonNull = lombok.NonNull
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports org.nd4j.common.primitives

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

Namespace org.deeplearning4j.datasets.iterator

	<Serializable>
	Public Class INDArrayDataSetIterator
		Inherits AbstractDataSetIterator(Of INDArray)

		''' <param name="iterable">  Iterable to source data from </param>
		''' <param name="batchSize"> Batch size for generated DataSet objects </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public INDArrayDataSetIterator(@NonNull Iterable<org.nd4j.common.primitives.Pair<org.nd4j.linalg.api.ndarray.INDArray, org.nd4j.linalg.api.ndarray.INDArray>> iterable, int batchSize)
		Public Sub New(ByVal iterable As IEnumerable(Of Pair(Of INDArray, INDArray)), ByVal batchSize As Integer)
			MyBase.New(iterable, batchSize)
		End Sub
	End Class

End Namespace