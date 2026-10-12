Imports System
Imports Preconditions = org.nd4j.common.base.Preconditions
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

Namespace org.nd4j.linalg.dataset.api.preprocessor

	<Serializable>
	Public Class CompositeDataSetPreProcessor
		Implements DataSetPreProcessor

		Private ReadOnly stopOnEmptyDataSet As Boolean
		Private preProcessors() As DataSetPreProcessor

		''' <param name="preProcessors"> Preprocessors to apply. They will be applied in this order </param>
		Public Sub New(ParamArray ByVal preProcessors() As DataSetPreProcessor)
			Me.New(False, preProcessors)
		End Sub

		Public Sub New(ByVal stopOnEmptyDataSet As Boolean, ParamArray ByVal preProcessors() As DataSetPreProcessor)
			Me.stopOnEmptyDataSet = stopOnEmptyDataSet
			Me.preProcessors = preProcessors
		End Sub

		Public Overridable Sub preProcess(ByVal dataSet As DataSet)
			Preconditions.checkNotNull(dataSet, "Encountered null dataSet")

			If stopOnEmptyDataSet AndAlso dataSet.Empty Then
				Return
			End If

			For Each p As DataSetPreProcessor In preProcessors
				p.preProcess(dataSet)

				If stopOnEmptyDataSet AndAlso dataSet.Empty Then
					Return
				End If
			Next p
		End Sub
	End Class

End Namespace