Imports AllArgsConstructor = lombok.AllArgsConstructor
Imports [Function] = org.apache.spark.api.java.function.Function
Imports org.nd4j.common.loader
Imports Source = org.nd4j.common.loader.Source
Imports SourceFactory = org.nd4j.common.loader.SourceFactory
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

Namespace org.deeplearning4j.spark.impl.common


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @AllArgsConstructor public class LoadDataSetFunction implements org.apache.spark.api.java.function.@Function<String, org.nd4j.linalg.dataset.DataSet>
	Public Class LoadDataSetFunction
		Implements [Function](Of String, DataSet)

		Private ReadOnly loader As Loader(Of DataSet)
		Private ReadOnly factory As SourceFactory

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public org.nd4j.linalg.dataset.DataSet call(String path) throws Exception
		Public Overrides Function [call](ByVal path As String) As DataSet
			Dim s As Source = factory.getSource(path)
			Return loader.load(s)
		End Function
	End Class

End Namespace