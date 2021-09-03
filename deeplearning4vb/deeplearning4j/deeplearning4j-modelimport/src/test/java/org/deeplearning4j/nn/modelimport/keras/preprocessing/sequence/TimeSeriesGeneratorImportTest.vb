Imports BaseDL4JTest = org.deeplearning4j.BaseDL4JTest
Imports InvalidKerasConfigurationException = org.deeplearning4j.nn.modelimport.keras.exceptions.InvalidKerasConfigurationException
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports Timeout = org.junit.jupiter.api.Timeout
Imports Resources = org.nd4j.common.resources.Resources
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames

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

Namespace org.deeplearning4j.nn.modelimport.keras.preprocessing.sequence


	''' <summary>
	''' Import test for Keras TimeSeriesGenerator
	''' 
	''' @author Max Pumperla
	''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Tag(TagNames.FILE_IO) @Tag(TagNames.KERAS) @NativeTag public class TimeSeriesGeneratorImportTest extends org.deeplearning4j.BaseDL4JTest
	Public Class TimeSeriesGeneratorImportTest
		Inherits BaseDL4JTest

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test() @Timeout(300000) public void importTimeSeriesTest() throws IOException, org.deeplearning4j.nn.modelimport.keras.exceptions.InvalidKerasConfigurationException
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub importTimeSeriesTest()
			Dim path As String = "modelimport/keras/preprocessing/timeseries_generator.json"

			Dim gen As TimeSeriesGenerator = TimeSeriesGenerator.fromJson(Resources.asFile(path).getAbsolutePath())
		End Sub
	End Class

End Namespace