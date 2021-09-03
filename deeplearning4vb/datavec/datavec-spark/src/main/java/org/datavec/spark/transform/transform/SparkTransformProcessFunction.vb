﻿Imports System.Collections.Generic
Imports FlatMapFunction = org.apache.spark.api.java.function.FlatMapFunction
Imports TransformProcess = org.datavec.api.transform.TransformProcess
Imports Writable = org.datavec.api.writable.Writable

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

Namespace org.datavec.spark.transform.transform


	Public Class SparkTransformProcessFunction
		Implements FlatMapFunction(Of IList(Of Writable), IList(Of Writable))

		Private ReadOnly transformProcess As TransformProcess

		Public Sub New(ByVal transformProcess As TransformProcess)
			Me.transformProcess = transformProcess
		End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public java.util.Iterator<java.util.List<org.datavec.api.writable.Writable>> call(java.util.List<org.datavec.api.writable.Writable> v1) throws Exception
		Public Overrides Function [call](ByVal v1 As IList(Of Writable)) As IEnumerator(Of IList(Of Writable))
			Dim newList As IList(Of Writable) = transformProcess.execute(v1)
			If newList Is Nothing Then
				Return Collections.emptyIterator() 'Example was filtered out
			Else
				Return Collections.singletonList(newList).GetEnumerator()
			End If
		End Function

	End Class

End Namespace