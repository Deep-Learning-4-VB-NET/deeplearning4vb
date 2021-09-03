Imports System
Imports AllArgsConstructor = lombok.AllArgsConstructor
Imports Data = lombok.Data

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

Namespace org.nd4j.evaluation.meta

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @AllArgsConstructor @Data public class Prediction
	Public Class Prediction

		Private actualClass As Integer
		Private predictedClass As Integer
		Private recordMetaData As Object

		Public Overrides Function ToString() As String
			Return "Prediction(actualClass=" & actualClass & ",predictedClass=" & predictedClass & ",RecordMetaData=" & recordMetaData & ")"
		End Function

		''' <summary>
		''' Convenience method for getting the record meta data as a particular class (as an alternative to casting it manually).
		''' NOTE: This uses an unchecked cast inernally.
		''' </summary>
		''' <param name="recordMetaDataClass"> Class of the record metadata </param>
		''' @param <T>                 Type to return </param>
		Public Overridable Function getRecordMetaData(Of T)(ByVal recordMetaDataClass As Type(Of T)) As T
			Return DirectCast(recordMetaData, T)
		End Function
	End Class

End Namespace