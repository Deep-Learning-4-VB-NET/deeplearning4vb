Imports System
Imports JsonMappers = org.nd4j.serde.json.JsonMappers
Imports JsonTypeInfo = org.nd4j.shade.jackson.annotation.JsonTypeInfo
Imports JsonProcessingException = org.nd4j.shade.jackson.core.JsonProcessingException

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

Namespace org.nd4j.evaluation.curves


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @JsonTypeInfo(use = JsonTypeInfo.Id.@CLASS, include = JsonTypeInfo.@As.@PROPERTY) public abstract class BaseHistogram
	Public MustInherit Class BaseHistogram

		Public MustOverride ReadOnly Property Title As String

		Public MustOverride Function numPoints() As Integer

		Public MustOverride ReadOnly Property BinCounts As Integer()

		Public MustOverride ReadOnly Property BinLowerBounds As Double()

		Public MustOverride ReadOnly Property BinUpperBounds As Double()

		Public MustOverride ReadOnly Property BinMidValues As Double()


		''' <returns>  JSON representation of the curve </returns>
		Public Overridable Function toJson() As String
			Try
				Return JsonMappers.Mapper.writeValueAsString(Me)
			Catch e As JsonProcessingException
				Throw New Exception(e)
			End Try
		End Function

		''' <returns> YAML  representation of the curve </returns>
		Public Overridable Function toYaml() As String
			Try
				Return JsonMappers.YamlMapper.writeValueAsString(Me)
			Catch e As JsonProcessingException
				Throw New Exception(e)
			End Try
		End Function

		''' 
		''' <param name="json">       JSON representation </param>
		''' <param name="curveClass"> Class for the curve </param>
		''' @param <T>        Type </param>
		''' <returns>           Instance of the curve </returns>
		Public Shared Function fromJson(Of T As BaseHistogram)(ByVal json As String, ByVal curveClass As Type(Of T)) As T
			Try
				Return JsonMappers.Mapper.readValue(json, curveClass)
			Catch e As IOException
				Throw New Exception(e)
			End Try
		End Function

		''' 
		''' <param name="yaml">       YAML representation </param>
		''' <param name="curveClass"> Class for the curve </param>
		''' @param <T>        Type </param>
		''' <returns>           Instance of the curve </returns>
		Public Shared Function fromYaml(Of T As BaseHistogram)(ByVal yaml As String, ByVal curveClass As Type(Of T)) As T
			Try
				Return JsonMappers.YamlMapper.readValue(yaml, curveClass)
			Catch e As IOException
				Throw New Exception(e)
			End Try
		End Function

	End Class

End Namespace