Imports System
Imports System.Collections.Generic
Imports Microsoft.VisualBasic
Imports Data = lombok.Data
Imports EqualsAndHashCode = lombok.EqualsAndHashCode
Imports ColumnAnalysis = org.datavec.api.transform.analysis.columns.ColumnAnalysis
Imports SequenceLengthAnalysis = org.datavec.api.transform.analysis.sequence.SequenceLengthAnalysis
Imports Schema = org.datavec.api.transform.schema.Schema
Imports JsonMappers = org.datavec.api.transform.serde.JsonMappers
Imports JsonSerializer = org.datavec.api.transform.serde.JsonSerializer
Imports YamlSerializer = org.datavec.api.transform.serde.YamlSerializer
Imports InvalidTypeIdException = org.nd4j.shade.jackson.databind.exc.InvalidTypeIdException

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

Namespace org.datavec.api.transform.analysis


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @EqualsAndHashCode(callSuper = true) @Data public class SequenceDataAnalysis extends DataAnalysis
	<Serializable>
	Public Class SequenceDataAnalysis
		Inherits DataAnalysis

		Private sequenceLengthAnalysis As SequenceLengthAnalysis

		Public Sub New(ByVal schema As Schema, ByVal columnAnalysis As IList(Of ColumnAnalysis), ByVal sequenceAnalysis As SequenceLengthAnalysis)
			MyBase.New(schema, columnAnalysis)
			Me.sequenceLengthAnalysis = sequenceAnalysis
		End Sub

		Protected Friend Sub New()
			'No arg for JSON
		End Sub

		Public Shared Function fromJson(ByVal json As String) As SequenceDataAnalysis
			Try
				Return (New JsonSerializer()).ObjectMapper.readValue(json, GetType(SequenceDataAnalysis))
			Catch e As InvalidTypeIdException
				If e.Message.contains("@class") Then
					Try
						'JSON may be legacy (1.0.0-alpha or earlier), attempt to load it using old format
						Return JsonMappers.LegacyMapper.readValue(json, GetType(SequenceDataAnalysis))
					Catch e2 As IOException
						Throw New Exception(e2)
					End Try
				End If
				Throw New Exception(e)
			Catch e As IOException
				Throw New Exception(e)
			End Try
		End Function

		Public Shared Function fromYaml(ByVal yaml As String) As SequenceDataAnalysis
			Try
				Return (New YamlSerializer()).ObjectMapper.readValue(yaml, GetType(SequenceDataAnalysis))
			Catch e As IOException
				Throw New Exception(e)
			End Try
		End Function

		Public Overrides Function ToString() As String
			Return sequenceLengthAnalysis & vbLf & MyBase.ToString()
		End Function
	End Class

End Namespace