Imports org.deeplearning4j.earlystopping.saver
Imports LocalFileGraphSaver = org.deeplearning4j.earlystopping.saver.LocalFileGraphSaver
Imports LocalFileModelSaver = org.deeplearning4j.earlystopping.saver.LocalFileModelSaver
Imports Model = org.deeplearning4j.nn.api.Model
Imports JsonInclude = org.nd4j.shade.jackson.annotation.JsonInclude
Imports JsonSubTypes = org.nd4j.shade.jackson.annotation.JsonSubTypes
Imports JsonTypeInfo = org.nd4j.shade.jackson.annotation.JsonTypeInfo

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

Namespace org.deeplearning4j.earlystopping


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @JsonInclude(JsonInclude.Include.NON_NULL) @JsonSubTypes(value = {@JsonSubTypes.Type(value = InMemoryModelSaver.class, name = "InMemoryModelSaver"), @JsonSubTypes.Type(value = LocalFileGraphSaver.class, name = "LocalFileGraphSaver"), @JsonSubTypes.Type(value = LocalFileModelSaver.class, name = "LocalFileModelSaver")}) @JsonTypeInfo(use = JsonTypeInfo.Id.@CLASS, include = JsonTypeInfo.@As.@PROPERTY, property = "@class") public interface EarlyStoppingModelSaver<T extends org.deeplearning4j.nn.api.Model> extends java.io.Serializable
	Public Interface EarlyStoppingModelSaver(Of T As org.deeplearning4j.nn.api.Model)

		''' <summary>
		''' Save the best model (so far) learned during early stopping training </summary>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: void saveBestModel(T net, double score) throws java.io.IOException;
		Sub saveBestModel(ByVal net As T, ByVal score As Double)

		''' <summary>
		''' Save the latest (most recent) model learned during early stopping </summary>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: void saveLatestModel(T net, double score) throws java.io.IOException;
		Sub saveLatestModel(ByVal net As T, ByVal score As Double)

		''' <summary>
		''' Retrieve the best model that was previously saved </summary>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: T getBestModel() throws java.io.IOException;
		ReadOnly Property BestModel As T

		''' <summary>
		''' Retrieve the most recent model that was previously saved </summary>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: T getLatestModel() throws java.io.IOException;
		ReadOnly Property LatestModel As T

	End Interface

End Namespace