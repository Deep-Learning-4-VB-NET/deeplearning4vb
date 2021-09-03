Imports System.Collections.Generic
Imports NonNull = lombok.NonNull
Imports TokenPreProcess = org.deeplearning4j.text.tokenization.tokenizer.TokenPreProcess
Imports Preconditions = org.nd4j.common.base.Preconditions

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

Namespace org.deeplearning4j.text.tokenization.tokenizer.preprocessor


	Public Class CompositePreProcessor
		Implements TokenPreProcess

		Private preProcessors As IList(Of TokenPreProcess)

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public CompositePreProcessor(@NonNull TokenPreProcess... preProcessors)
		Public Sub New(ParamArray ByVal preProcessors() As TokenPreProcess)
			Preconditions.checkState(preProcessors.Length > 0, "No preprocessors were specified (empty input)")
			Me.preProcessors = New List(Of TokenPreProcess) From {preProcessors}
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public CompositePreProcessor(@NonNull Collection<? extends org.deeplearning4j.text.tokenization.tokenizer.TokenPreProcess> preProcessors)
'JAVA TO VB CONVERTER TODO TASK: Wildcard generics in method parameters are not converted:
		Public Sub New(ByVal preProcessors As ICollection(Of TokenPreProcess))
			Preconditions.checkState(Not preProcessors.isEmpty(), "No preprocessors were specified (empty input)")
			Me.preProcessors = New List(Of TokenPreProcess)(preProcessors)
		End Sub

		Public Overridable Function preProcess(ByVal token As String) As String Implements TokenPreProcess.preProcess
			Dim s As String = token
			For Each tpp As TokenPreProcess In preProcessors
				s = tpp.preProcess(s)
			Next tpp
			Return s
		End Function
	End Class

End Namespace