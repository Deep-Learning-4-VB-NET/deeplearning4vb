Imports System.Collections.Generic
Imports LongWritable = org.datavec.api.writable.LongWritable
Imports Writable = org.datavec.api.writable.Writable
Imports org.nd4j.common.function
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

Namespace org.datavec.local.transforms.rank


	Public Class UnzipForCalculateSortedRankFunction
		Implements [Function](Of Pair(Of Pair(Of Writable, IList(Of Writable)), Long), IList(Of Writable))

		Public Overridable Function apply(ByVal v1 As Pair(Of Pair(Of Writable, IList(Of Writable)), Long)) As IList(Of Writable)
			Dim inputWritables As IList(Of Writable) = New List(Of Writable)(v1.First.Second)
			inputWritables.Add(New LongWritable(v1.Second))
			Return inputWritables
		End Function
	End Class

End Namespace