Imports Builder = lombok.Builder
Imports Data = lombok.Data
Imports Accessors = lombok.experimental.Accessors
Imports org.nd4j.common.function

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
Namespace org.nd4j.autodiff.listeners.profiler.comparison


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Data @Accessors(fluent = true) @Builder public class Config
	Public Class Config

		Private p1Name As String
		Private p2Name As String
		Private profile1 As File
		Private profile2 As File
		Private profile1IsDir As Boolean
		Private profile2IsDir As Boolean
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Builder.@Default private ProfileAnalyzer.ProfileFormat profile1Format = ProfileAnalyzer.ProfileFormat.SAMEDIFF;
		Private profile1Format As ProfileAnalyzer.ProfileFormat = ProfileAnalyzer.ProfileFormat.SAMEDIFF
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Builder.@Default private ProfileAnalyzer.ProfileFormat profile2Format = ProfileAnalyzer.ProfileFormat.SAMEDIFF;
		Private profile2Format As ProfileAnalyzer.ProfileFormat = ProfileAnalyzer.ProfileFormat.SAMEDIFF
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Builder.@Default private ProfileAnalyzer.SortBy sortBy = ProfileAnalyzer.SortBy.PROFILE1_PC;
		Private sortBy As ProfileAnalyzer.SortBy = ProfileAnalyzer.SortBy.PROFILE1_PC
		Private filter As BiFunction(Of OpStats, OpStats, Boolean) 'Return true to keep, false to remove
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Builder.@Default private ProfileAnalyzer.OutputFormat format = ProfileAnalyzer.OutputFormat.TEXT;
		Private format As ProfileAnalyzer.OutputFormat = ProfileAnalyzer.OutputFormat.TEXT

	End Class

End Namespace