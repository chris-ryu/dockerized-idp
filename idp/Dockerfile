FROM unicon/shibboleth-idp:latest

MAINTAINER Chris Ryu

ENV JAVA_HOME "/opt/jre-home"
RUN yum install -y wget

COPY shibboleth-idp/ /opt/shibboleth-idp/
COPY shib-jetty-base/ /opt/shib-jetty-base/
RUN wget -P /opt/shibboleth-idp/webapp/WEB-INF/lib/ https://repo1.maven.org/maven2/com/oracle/database/jdbc/ojdbc8/19.6.0.0/ojdbc8-19.6.0.0.jar
# RUN  yum update -y \
#     && yum install -y vim \
#     && yum clean all