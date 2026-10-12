Imports System
Imports System.IO
Imports InputStreamCreator = org.deeplearning4j.models.word2vec.InputStreamCreator
Imports DocumentIterator = org.deeplearning4j.text.documentiterator.DocumentIterator

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

Namespace org.deeplearning4j.bagofwords.vectorizer


	<Serializable>
	Public Class DefaultInputStreamCreator
		Implements InputStreamCreator

		Private iter As DocumentIterator

		Public Sub New(ByVal iter As DocumentIterator)
			Me.iter = iter
		End Sub

		Public Overridable Function create() As Stream Implements InputStreamCreator.create
			Return iter.nextDocument()
		End Function
	End Class

End Namespace