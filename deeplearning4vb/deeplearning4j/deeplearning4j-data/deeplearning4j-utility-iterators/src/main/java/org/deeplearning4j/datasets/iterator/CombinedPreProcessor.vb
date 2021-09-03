Imports System
Imports System.Collections.Generic
Imports NonNull = lombok.NonNull
Imports DataSet = org.nd4j.linalg.dataset.api.DataSet
Imports DataSetPreProcessor = org.nd4j.linalg.dataset.api.DataSetPreProcessor

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


	<Serializable>
	Public Class CombinedPreProcessor
		Implements DataSetPreProcessor

		Private preProcessors As IList(Of DataSetPreProcessor)

		Private Sub New()

		End Sub

		''' <summary>
		''' Pre process a dataset sequentially
		''' </summary>
		''' <param name="toPreProcess"> the data set to pre process </param>
		Public Overridable Sub preProcess(ByVal toPreProcess As DataSet)
			For Each preProcessor As DataSetPreProcessor In preProcessors
				preProcessor.preProcess(toPreProcess)
			Next preProcessor
		End Sub

		Public Class Builder
			Friend preProcessors As IList(Of DataSetPreProcessor) = New List(Of DataSetPreProcessor)()

			Public Sub New()

			End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public Builder addPreProcessor(@NonNull DataSetPreProcessor preProcessor)
			Public Overridable Function addPreProcessor(ByVal preProcessor As DataSetPreProcessor) As Builder
				preProcessors.Add(preProcessor)
				Return Me
			End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public Builder addPreProcessor(int idx, @NonNull DataSetPreProcessor preProcessor)
			Public Overridable Function addPreProcessor(ByVal idx As Integer, ByVal preProcessor As DataSetPreProcessor) As Builder
				preProcessors.Insert(idx, preProcessor)
				Return Me
			End Function


			Public Overridable Function build() As CombinedPreProcessor
				Dim preProcessor As New CombinedPreProcessor()
				preProcessor.preProcessors = Me.preProcessors
				Return preProcessor
			End Function
		End Class
	End Class

End Namespace