Imports System.Collections.Generic
Imports [Function] = org.apache.spark.api.java.function.Function
Imports LongWritable = org.datavec.api.writable.LongWritable
Imports Writable = org.datavec.api.writable.Writable
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

Namespace org.datavec.spark.transform.rank


	Public Class UnzipForCalculateSortedRankFunction
		Implements [Function](Of Tuple2(Of Tuple2(Of Writable, IList(Of Writable)), Long), IList(Of Writable))

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public java.util.List<org.datavec.api.writable.Writable> call(scala.Tuple2<scala.Tuple2<org.datavec.api.writable.Writable, java.util.List<org.datavec.api.writable.Writable>>, Long> v1) throws Exception
		Public Overrides Function [call](ByVal v1 As Tuple2(Of Tuple2(Of Writable, IList(Of Writable)), Long)) As IList(Of Writable)
			Dim inputWritables As IList(Of Writable) = New List(Of Writable)(v1._1()._2())
			inputWritables.Add(New LongWritable(v1._2()))
			Return inputWritables
		End Function
	End Class

End Namespace