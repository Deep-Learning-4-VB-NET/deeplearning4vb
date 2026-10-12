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

Namespace org.nd4j.linalg.dataset.api.preprocessor

	Public Class CompositeMultiDataSetPreProcessor
		Implements MultiDataSetPreProcessor

		Private preProcessors() As MultiDataSetPreProcessor

		''' <param name="preProcessors"> Preprocessors to apply. They will be applied in this order </param>
		Public Sub New(ParamArray ByVal preProcessors() As MultiDataSetPreProcessor)
			Me.preProcessors = preProcessors
		End Sub

		Public Overridable Sub preProcess(ByVal multiDataSet As MultiDataSet)
			For Each p As MultiDataSetPreProcessor In preProcessors
				p.preProcess(multiDataSet)
			Next p
		End Sub
	End Class

End Namespace