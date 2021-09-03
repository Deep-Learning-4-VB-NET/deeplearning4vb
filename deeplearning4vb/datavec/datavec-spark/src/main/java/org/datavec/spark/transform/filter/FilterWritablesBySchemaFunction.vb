Imports [Function] = org.apache.spark.api.java.function.Function
Imports ColumnMetaData = org.datavec.api.transform.metadata.ColumnMetaData
Imports NullWritable = org.datavec.api.writable.NullWritable
Imports Text = org.datavec.api.writable.Text
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

Namespace org.datavec.spark.transform.filter

	Public Class FilterWritablesBySchemaFunction
		Implements [Function](Of Writable, Boolean)

		Private ReadOnly meta As ColumnMetaData
		Private ReadOnly keepValid As Boolean 'If true: keep valid. If false: keep invalid
		Private ReadOnly excludeMissing As Boolean 'If true: remove/exclude any


		Public Sub New(ByVal meta As ColumnMetaData, ByVal keepValid As Boolean)
			Me.New(meta, keepValid, False)
		End Sub

		''' 
		''' <param name="meta">              Column meta data </param>
		''' <param name="keepValid">         If true: keep only the valid writables. If false: keep only the invalid writables </param>
		''' <param name="excludeMissing">    If true: don't return any missing values, regardless of keepValid setting (i.e., exclude any NullWritable or empty string values) </param>
		Public Sub New(ByVal meta As ColumnMetaData, ByVal keepValid As Boolean, ByVal excludeMissing As Boolean)
			Me.meta = meta
			Me.keepValid = keepValid
			Me.excludeMissing = excludeMissing
		End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public System.Nullable<Boolean> call(org.datavec.api.writable.Writable v1) throws Exception
		Public Overrides Function [call](ByVal v1 As Writable) As Boolean?
			Dim valid As Boolean = meta.isValid(v1)
			If excludeMissing AndAlso (TypeOf v1 Is NullWritable OrElse TypeOf v1 Is Text AndAlso (String.ReferenceEquals(v1.ToString(), Nothing) OrElse v1.ToString().Length = 0)) Then
				Return False 'Remove (spark)
			End If
			If keepValid Then
				Return valid 'Spark: return true to keep
			Else
				Return Not valid
			End If
		End Function
	End Class

End Namespace