Imports System
Imports System.Collections.Generic
Imports AllArgsConstructor = lombok.AllArgsConstructor
Imports Tuple2 = scala.Tuple2

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

Namespace org.datavec.spark.transform.misc.comparator


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @AllArgsConstructor public class Tuple2Comparator<T> implements java.util.Comparator<scala.Tuple2<T, Long>>, java.io.Serializable
	<Serializable>
	Public Class Tuple2Comparator(Of T)
		Implements IComparer(Of Tuple2(Of T, Long))

		Private ReadOnly ascending As Boolean

		Public Overridable Function Compare(ByVal o1 As Tuple2(Of T, Long), ByVal o2 As Tuple2(Of T, Long)) As Integer Implements IComparer(Of Tuple2(Of T, Long)).Compare
			If ascending Then
				Return Long.compare(o1._2(), o2._2)
			Else
				Return -Long.compare(o1._2(), o2._2)
			End If
		End Function
	End Class

End Namespace