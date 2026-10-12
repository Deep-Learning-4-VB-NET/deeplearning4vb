Imports System
Imports System.IO
Imports DataSetLoader = org.deeplearning4j.core.loader.DataSetLoader
Imports Source = org.nd4j.common.loader.Source
Imports DataSet = org.nd4j.linalg.dataset.DataSet

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

Namespace org.deeplearning4j.core.loader.impl


	<Serializable>
	Public Class SerializedDataSetLoader
		Implements DataSetLoader

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public org.nd4j.linalg.dataset.DataSet load(org.nd4j.common.loader.Source source) throws java.io.IOException
		Public Overridable Function load(ByVal source As Source) As DataSet
			Dim ds As New DataSet()
			Using [is] As Stream = source.InputStream
				ds.load([is])
			End Using
			Return ds
		End Function
	End Class

End Namespace