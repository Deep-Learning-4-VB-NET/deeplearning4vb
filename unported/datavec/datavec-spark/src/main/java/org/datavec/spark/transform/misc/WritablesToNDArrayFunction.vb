Imports System.Collections.Generic
Imports [Function] = org.apache.spark.api.java.function.Function
Imports NDArrayWritable = org.datavec.api.writable.NDArrayWritable
Imports Writable = org.datavec.api.writable.Writable
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports NDArrayIndex = org.nd4j.linalg.indexing.NDArrayIndex

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

Namespace org.datavec.spark.transform.misc


	Public Class WritablesToNDArrayFunction
		Implements [Function](Of IList(Of Writable), INDArray)

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public org.nd4j.linalg.api.ndarray.INDArray call(java.util.List<org.datavec.api.writable.Writable> c) throws Exception
		Public Overrides Function [call](ByVal c As IList(Of Writable)) As INDArray
			Dim length As Integer = 0
			For Each w As Writable In c
				If TypeOf w Is NDArrayWritable Then
					Dim a As INDArray = DirectCast(w, NDArrayWritable).get()
					If a.RowVector Then
						length += a.columns()
					Else
						Throw New System.NotSupportedException("NDArrayWritable is not a row vector." & " Can only concat row vectors with other writables. Shape: " & Arrays.toString(a.shape()))
					End If
				Else
					length += 1
				End If
			Next w

			Dim arr As INDArray = Nd4j.zeros(DataType.FLOAT, 1, length)
			Dim idx As Integer = 0
			For Each w As Writable In c
				If TypeOf w Is NDArrayWritable Then
					Dim subArr As INDArray = DirectCast(w, NDArrayWritable).get()
					Dim subLength As Integer = subArr.columns()
					arr.get(NDArrayIndex.point(0), NDArrayIndex.interval(idx, idx + subLength)).assign(subArr)
					idx += subLength
				Else
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
'ORIGINAL LINE: arr.putScalar(idx++, w.toDouble());
					arr.putScalar(idx, w.toDouble())
						idx += 1
				End If
			Next w

			Return arr
		End Function
	End Class

End Namespace