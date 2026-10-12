Imports System.Collections.Generic
Imports NonNull = lombok.NonNull
Imports MultiDataSet = org.nd4j.linalg.dataset.api.MultiDataSet
Imports MultiDataSetPreProcessor = org.nd4j.linalg.dataset.api.MultiDataSetPreProcessor

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

Namespace org.deeplearning4j.datasets.iterator


	Public Class CombinedMultiDataSetPreProcessor
		Implements MultiDataSetPreProcessor

		Private preProcessors As IList(Of MultiDataSetPreProcessor)

		Private Sub New()

		End Sub

		Public Overridable Sub preProcess(ByVal multiDataSet As MultiDataSet)
			For Each preProcessor As MultiDataSetPreProcessor In preProcessors
				preProcessor.preProcess(multiDataSet)
			Next preProcessor
		End Sub

		Public Class Builder
			Friend preProcessors As IList(Of MultiDataSetPreProcessor) = New List(Of MultiDataSetPreProcessor)()

			Public Sub New()

			End Sub

			''' <param name="preProcessor"> to be added to list of preprocessors to be applied </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public Builder addPreProcessor(@NonNull MultiDataSetPreProcessor preProcessor)
			Public Overridable Function addPreProcessor(ByVal preProcessor As MultiDataSetPreProcessor) As Builder
				preProcessors.Add(preProcessor)
				Return Me
			End Function

			''' <summary>
			''' Inserts the specified preprocessor at the specified position to the list of preprocessors to be applied </summary>
			''' <param name="idx"> the position to apply the specified preprocessor at </param>
			''' <param name="preProcessor"> to be added to list of preprocessors to be applied </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public Builder addPreProcessor(int idx, @NonNull MultiDataSetPreProcessor preProcessor)
			Public Overridable Function addPreProcessor(ByVal idx As Integer, ByVal preProcessor As MultiDataSetPreProcessor) As Builder
				preProcessors.Insert(idx, preProcessor)
				Return Me
			End Function

			Public Overridable Function build() As CombinedMultiDataSetPreProcessor
				Dim preProcessor As New CombinedMultiDataSetPreProcessor()
				preProcessor.preProcessors = Me.preProcessors
				Return preProcessor
			End Function
		End Class
	End Class

End Namespace