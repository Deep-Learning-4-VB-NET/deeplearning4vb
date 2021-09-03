﻿Imports System
Imports System.Collections.Generic
Imports AllArgsConstructor = lombok.AllArgsConstructor
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports [Function] = org.apache.spark.api.java.function.Function
Imports Transform = org.datavec.api.transform.Transform
Imports Writable = org.datavec.api.writable.Writable
Imports SparkTransformExecutor = org.datavec.spark.transform.SparkTransformExecutor

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


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @AllArgsConstructor @Slf4j public class SparkTransformFunction implements org.apache.spark.api.java.function.@Function<java.util.List<org.datavec.api.writable.Writable>, java.util.List<org.datavec.api.writable.Writable>>
	Public Class SparkTransformFunction
		Implements [Function](Of IList(Of Writable), IList(Of Writable))

		Private ReadOnly transform As Transform

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public java.util.List<org.datavec.api.writable.Writable> call(java.util.List<org.datavec.api.writable.Writable> v1) throws Exception
		Public Overrides Function [call](ByVal v1 As IList(Of Writable)) As IList(Of Writable)
			If SparkTransformExecutor.TryCatch Then
				Try
					Return transform.map(v1)
				Catch e As Exception
					log.warn("Error occurred " & e & " on record " & v1)
					Return New List(Of Writable)()
				End Try
			End If
			Return transform.map(v1)
		End Function
	End Class

End Namespace